using Application.Abstractions.Repositories;
using Application.Common.Enums;
using Application.Dto.ChatExercise;
using Application.Dto.Exercise;
using Application.Models;
using Dapper;
using Infrastructure.Postgres.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Npgsql;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Postgres.Main.Repositories;

public class StudentExerciseRepositoryPostgres : IStudentExerciseRepository
{
    const int STATUS = 0;
    const int MARK = 1;
    const int THREAD = 2;

    private readonly MainDbContext _mainDbContext;
    private readonly AppIdentityDbContext _identityDbContext;


    public StudentExerciseRepositoryPostgres(MainDbContext mainDbContext, AppIdentityDbContext identityDbContext)
    {
        _mainDbContext = mainDbContext;
        _identityDbContext = identityDbContext;
    }

    public Task<StudentExerciseDto?> GetStudentExerciseAsync(Guid exerciseId, string userId, CancellationToken ct)
    {
        return _mainDbContext.StudentExercises
            .AsNoTracking()
            .Where(x => x.ExerciseId == exerciseId && x.StudentId == userId)
            .Select(rec => new StudentExerciseDto(rec.Status, rec.Mark, rec.ThreadId))
            .SingleOrDefaultAsync(ct);
    }

    public Task<StudentExerciseDto?> GetStudentExerciseForMentorAsync(Guid exerciseId, string mentorId,
        string studentId, CancellationToken ct)
    {
        return _mainDbContext.StudentExercises
            .AsNoTracking()
            .Where(x => x.ExerciseId == exerciseId && x.StudentId == studentId && x.AssignedMentorId == mentorId)
            .Select(rec => new StudentExerciseDto(rec.Status, rec.Mark, rec.ThreadId))
            .SingleOrDefaultAsync(ct);
    }

    /// <summary>
    /// UPSERT-версия без исключений 23505:
    /// - делает INSERT ... ON CONFLICT ... DO UPDATE (no-op)
    /// - всегда возвращает Status/Mark/ThreadId через RETURNING (и при INSERT, и при конфликте)
    /// </summary>
    public async Task<StudentExerciseDto> TryInsertNewStudentExerciseOrReadExistingAsync(StudentExercise newEntity,
        CancellationToken ct)
    {
        // Важно: DO UPDATE делает "no-op" (перезаписывает ThreadId самим собой),
        // чтобы RETURNING сработал и при конфликте, не меняя реальные данные.
        const string sql = """
                INSERT INTO main.student_exercises
                    ("Id","StudentId","ExerciseId","CourseId","Status","ThreadId")
                VALUES
                    (@id,@studentId,@exerciseId,@courseId,@status,@threadId)
                ON CONFLICT ("StudentId","ExerciseId")
                DO UPDATE SET "ThreadId" = main.student_exercises."ThreadId"
                RETURNING "Status","Mark","ThreadId";
            """;

        var conn = (NpgsqlConnection)_mainDbContext.Database.GetDbConnection();

        var shouldClose = conn.State != ConnectionState.Open;
        if (shouldClose)
            await conn.OpenAsync(ct);

        try
        {
            await using var cmd = new NpgsqlCommand(sql, conn);
            // Генерируем новый Id, потому что entity создаётся без Id (Guid.Empty)
            cmd.Parameters.AddWithValue("id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("courseId", newEntity.CourseId);
            cmd.Parameters.AddWithValue("exerciseId", newEntity.ExerciseId);
            cmd.Parameters.AddWithValue("status", newEntity.Status.ToString());
            cmd.Parameters.AddWithValue("studentId", newEntity.StudentId);
            cmd.Parameters.AddWithValue("threadId", newEntity.ThreadId);

            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
            if (!await reader.ReadAsync(ct))
                throw new InvalidOperationException("[StudentExerciseRepository] UPSERT returned no row." +
                    " Check SQL or table constraints.");

            var status = Enum.Parse<StatusStudentExercise>(reader.GetString(STATUS));
            var mark = reader.IsDBNull(MARK) ? (int?)null : reader.GetInt32(MARK);
            var threadId = reader.GetString(THREAD);

            return new StudentExerciseDto(status, mark, threadId);
        }
        finally
        {
            if (shouldClose)
                await conn.CloseAsync();
        }
    }

    public async Task<HashSet<string>> GetUsedThreads(string[] checkedThreadIds)
    {
        var usedThreads = await _mainDbContext.StudentExercises
            .AsNoTracking()
            .Where(x => checkedThreadIds.Contains(x.ThreadId))
            .Select(x => x.ThreadId)
            .ToListAsync();

        return usedThreads.ToHashSet(StringComparer.Ordinal);
    }

    public Task<bool> IsUserThreadOwner(string userId, string threadId, CancellationToken ct)
    {
        return _mainDbContext.StudentExercises
            .AsNoTracking()
            .Where(x => x.StudentId == userId && x.ThreadId == threadId)
            .AnyAsync(ct);
    }

    public Task<Guid> GetExistingStudentExerciseIdAsync(Guid exerciseId, string studentId, CancellationToken ct)
    {
        return _mainDbContext.StudentExercises
            .AsNoTracking()
            .Where(x => x.StudentId == studentId && x.ExerciseId == exerciseId)
            .Select(x => x.Id)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<bool> TryAssignMentorToStudentExerciseAsync(Guid existingStudentExercise, string mentorId,
        CancellationToken ct)
    {
        var affectedRows = await _mainDbContext.StudentExercises
            .Where(x => x.Id == existingStudentExercise && x.AssignedMentorId == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.AssignedMentorId, mentorId), ct);

        return affectedRows == 1;
    }

    private async Task<List<MentorExerciseChatInboxResponse>> RunDapperSQLAsync(string sql,
        object? param, CancellationToken ct)
    {
        var conn = (NpgsqlConnection)_mainDbContext.Database.GetDbConnection();

        var shouldClose = conn.State != ConnectionState.Open;
        if (shouldClose)
            await conn.OpenAsync(ct);

        try
        {
            var rows = await conn.QueryAsync<MentorExerciseChatInboxResponse>(
                new CommandDefinition(sql, param, cancellationToken: ct));

            return rows.AsList();
        }
        finally
        {
            if (shouldClose)
                await conn.CloseAsync();
        }
    }

    public Task<List<MentorExerciseChatInboxResponse>> GetMentorExerciseChatInbox(CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                se."StudentId" || '_' || se."ExerciseId"      AS "StudentExerciseId",
                se."StudentId"                                AS "StudentId",
                u."Email"                                     AS "Email",
                se."ExerciseId"                               AS "ExerciseId",
                e."Title"                                     AS "ExerciseTitle",
                e."ShortDescription"                          AS "ExerciseShortDescription",
                se."Status"                                   AS "Status",
                se."Mark"                                     AS "Mark",
                se."RequestedCheckAt"                         AS "RequestedCheckAt",
                se."CheckedAt"                                AS "CheckedAt"
            FROM main.student_exercises se
            JOIN main.exercises e ON se."ExerciseId" = e."Id"
            JOIN identity."AspNetUsers" u ON se."StudentId" = u."Id"
            ORDER BY
                se."RequestedCheckAt" DESC NULLS LAST,
                se."StudentId" || '_' || se."ExerciseId"
            """;

        return RunDapperSQLAsync(sql, null, ct);
    }


    public Task<List<MentorExerciseChatInboxResponse>> GetMentorExerciseChatInbox(string? Email,
        bool? OnlyOnCheck, CancellationToken ct)
    {
        var sql = new StringBuilder("""
        SELECT
            se."StudentId" || '_' || se."ExerciseId"      AS "StudentExerciseId",
            se."StudentId"                                AS "StudentId",
            u."Email"                                     AS "Email",
            se."ExerciseId"                               AS "ExerciseId",
            e."Title"                                     AS "ExerciseTitle",
            e."ShortDescription"                          AS "ExerciseShortDescription",
            se."Status"                                   AS "Status",
            se."Mark"                                     AS "Mark",
            se."RequestedCheckAt"                         AS "RequestedCheckAt",
            se."CheckedAt"                                AS "CheckedAt"
        FROM main.student_exercises se
        JOIN main.exercises e ON se."ExerciseId" = e."Id"
        JOIN identity."AspNetUsers" u ON se."StudentId" = u."Id"
        """);

        var filters = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(Email))
        {
            filters.Add("""u."Email" ILIKE @Email""");
            parameters.Add("Email", $"%{Email.Trim()}%");
        }

        if (OnlyOnCheck == true)
        {
            filters.Add("""se."Status" = 'OnCheck'""");
        }

        if (filters.Count > 0)
        {
            sql.AppendLine("WHERE " + string.Join(" AND ", filters));
        }

        sql.AppendLine("""
        ORDER BY
            se."RequestedCheckAt" DESC NULLS LAST,
            se."StudentId" || '_' || se."ExerciseId"
        """);

        return RunDapperSQLAsync(sql.ToString(), parameters, ct);
    }

    public Task UpdateStatusAsync(string threadId, ExerciseChatRole chatRole, int? mark,
        StatusStudentExercise newStatus, CancellationToken ct)
    {
        var utcNow = DateTime.Now;

        var query = _mainDbContext.StudentExercises
            .Where(x => x.ThreadId == threadId);

        return chatRole switch
        {
            ExerciseChatRole.Mentor => query.ExecuteUpdateAsync(
                calls => calls
                    .SetProperty(x => x.Status, newStatus)
                    .SetProperty(x => x.CheckedAt, utcNow)
                    .SetProperty(x => x.Mark, mark),
                ct),

            ExerciseChatRole.Student => query.ExecuteUpdateAsync(
                calls => calls
                    .SetProperty(x => x.Status, newStatus)
                    .SetProperty(x => x.RequestedCheckAt, utcNow),
                ct),

            _ => throw new ArgumentOutOfRangeException(nameof(chatRole), chatRole, null)
        };
    }

    public Task<bool> HasAuthorWriteAccessAsync(string threadId, ExerciseChatRole authorRole, CancellationToken ct)
    {
        var correctThreadStatus = authorRole == ExerciseChatRole.Student ?
            StatusStudentExercise.OnStudent : StatusStudentExercise.OnMentor;

        return _mainDbContext.StudentExercises
            .AsNoTracking()
            .Where(x => x.ThreadId == threadId && x.Status == correctThreadStatus)
            .AnyAsync(ct);
    }
}

