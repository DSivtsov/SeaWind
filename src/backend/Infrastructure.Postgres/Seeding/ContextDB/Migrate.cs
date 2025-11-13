using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding.ContextDB;

internal class Migrate<TContext> where TContext : DbContext
{
    private readonly ILogger _log;
    private readonly TContext _db;

    public Migrate(TContext db, ILogger log)
    {
        _db = db;
        _log = log;
    }

    internal async Task<bool> Run(CancellationToken ct = default)
    {
        _log.LogInformation("Migration DbContext=[{DbContext}]...", typeof(TContext).Name);

        // Автоматическая миграция
        if (!await MigrateAsync(ct)) return false;

        return true;
    }

    private async Task<bool> MigrateAsync(CancellationToken ct)
    {
        try
        {
            await _db.Database.MigrateAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Error while Migration for {_db.Database.GetDbConnection().Database}");
            return false;
        }
    }
}