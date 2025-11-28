using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding.ContextDB;

internal class Checker<TContext> where TContext : DbContext
{
    private readonly ILogger _log;
    private readonly TContext _db;

    public Checker(TContext db, ILogger log)
    {
        _db = db;
        _log = log;
    }

    internal async Task<bool> Check(CancellationToken ct = default)
    {
        _log.LogInformation("Checking DbContext=[{DbContext}]...", typeof(TContext).Name);

        // Проверка подсоединения
        if (!await CanConnectAsync(ct)) return false;

        // Проверка отсутствия не выполненных миграций
        if (!await NoActiveMigrations(ct)) return false;

        // Проверка таблиц
        if (!await HasUserTablesAsync(ct)) return false;

        return true;
    }

    private async Task<bool> NoActiveMigrations(CancellationToken ct = default)
    {
        try
        {
            var pending = await _db.Database.GetPendingMigrationsAsync(ct);
            if (pending.Any())
            {
                // Custom Exception which can start Auto Migration
                throw new PendingMigrationsException(pending);
            }
            return true;
        }
        catch (PendingMigrationsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error while checking Pending Migrations [{DbContext}]...", typeof(TContext).Name);
            return false;
        }
    }

    private async Task<bool> CanConnectAsync(CancellationToken ct)
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(ct);
            if (!canConnect)
                _log.LogError($"Cannot connect to database: {_db.Database.GetDbConnection().Database}.");

            return canConnect;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Error while checking connection for {_db.Database.GetDbConnection().Database}");
            return false;
        }
    }

    private async Task<bool> HasUserTablesAsync(CancellationToken ct)
    {
        try
        {
            var connection = _db.Database.GetDbConnection();
            await connection.OpenAsync(ct);

            // Получаем текущую схему (если задана в контексте)
            //var schema = "public"; // значение по умолчанию
            var schema = _db.Model.GetDefaultSchema();
            if (schema is { Length: 0 })
            {
                _log.LogError("No Set a Schema for Table in DB.");
                return false;
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT COUNT(*)
                FROM information_schema.tables
                WHERE table_schema = @schema
                  AND table_type = 'BASE TABLE';";

            var p = cmd.CreateParameter();
            p.ParameterName = "@schema";
            p.Value = schema;
            cmd.Parameters.Add(p);

            var countTable = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
            if (countTable > 0)
                return true;

            _log.LogError("No Table in DB.");
            return false;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error while checking schema tables for [{DbContext}]...", typeof(TContext).Name);
            return false;
        }

    }
}