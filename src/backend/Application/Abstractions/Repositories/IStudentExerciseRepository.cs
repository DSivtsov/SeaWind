using Application.Common.Enums;
using Application.Dto.ChatExercise;
using Application.Dto.Exercise;
using Application.Models;

namespace Application.Abstractions.Repositories;

public interface IStudentExerciseRepository
{
    Task<HashSet<string>> GetUsedThreads(string[] checkedThreadIds);

    Task<StudentExerciseDto?> GetStudentExerciseAsync(Guid exerciseId, string userId, CancellationToken ct);

    Task<StudentExerciseDto?> GetStudentExerciseForMentorAsync(Guid exerciseId, string mentorId,
    string studentId, CancellationToken ct);

    Task<StudentExerciseDto> TryInsertNewStudentExerciseOrReadExistingAsync(StudentExercise newEntity,
        CancellationToken ct);

    Task<bool> IsUserThreadOwner(string userId, string threadId, CancellationToken ct);

    Task<Guid> GetExistingStudentExerciseIdAsync(Guid exerciseId, string studentId, CancellationToken ct);

    Task<bool> TryAssignMentorToStudentExerciseAsync(Guid existingStudentExercise, string mentorId,
        CancellationToken ct);

    Task<List<MentorExerciseChatInboxResponse>> GetMentorExerciseChatInbox(CancellationToken ct);

    Task<List<MentorExerciseChatInboxResponse>> GetMentorExerciseChatInbox(string? email, bool? onlyOnCheck,
        CancellationToken ct);

    Task UpdateStatusAsync(string threadId, ExerciseChatRole chatRole, int? mark, StatusStudentExercise newStatus,
        CancellationToken ct);

    Task<bool> HasAuthorWriteAccessAsync(string threadId, ExerciseChatRole authorRole, CancellationToken ct);
}
