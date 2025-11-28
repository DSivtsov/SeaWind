using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding.ContextDB;

internal sealed class Seeder<TContext> where TContext : DbContext
{
    private readonly IEnumerable<ISeeder<TContext>> _seeders;
    private readonly TContext _dbContext;
    private readonly IPathFile _getterpathDemoData;

    public Seeder(IServiceProvider sp, IEnumerable<ISeeder<TContext>> seeders, IPathFile getterpathDemoData)
    {
        _seeders = seeders.OrderBy(s => s.Order);
        _dbContext = sp.GetRequiredService<TContext>();
        _getterpathDemoData = getterpathDemoData;
    }

    internal async Task RunSeedingAsync(OptionsResult optRez, ILogger logSeeder, CancellationToken ct = default)
    {
        SeedInsertMode insertMode = optRez.InsertMode;
        string pathBase = optRez.PathBase;
        string pathVersion = optRez.PathVersion;

        foreach (ISeeder<TContext> seeder in _seeders)
        {
            logSeeder.LogInformation("Run Seeder [{Name}]", seeder.Name);

            try
            {
                string pathDemoDataFile;
                try
                {
                    // for support the "use two paths" option - pathBase & pathVersion
                    //pathDemoDataFile = _getterpathDemoData.Get(seeder.Name, pathBase, pathVersion);   
                    pathDemoDataFile = _getterpathDemoData.GetDataFile(seeder.Name, pathBase);
                }
                catch (Exception ex)
                {
                    logSeeder.LogError($"Aborted Seeding — Seeder [{seeder.Name}] : {ex.Message}");
                    continue;
                }

                var result = await seeder.LoadAndValidateAsync(pathDemoDataFile, ct);

                if (!result.ok)
                {
                    logSeeder.LogError($"Aborted Seeding — Seeder [{seeder.Name}] : {result.errorMsg}");
                    continue;
                }
            }
            // это не ошибка, а сигнал об отмене операции
            catch (OperationCanceledException)
            {
                throw;                    // пробрасываем дальше, чтобы отмена дошла до Host
            }
            // а в это уже обычная Unhandled ошибка
            catch (Exception ex)
            {
                logSeeder.LogError(ex, "Exception during [LoadAndValidateAsync] [{Name}]", seeder.Name);
            }

            if (!seeder.HasDemoData)
                throw new SeederDataException("LoadAndValidateAsync produced null demo data unexpectedly.");

            await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var result = await seeder.SeedAsync(_dbContext, insertMode, ct);

                if (!result.ok)
                {
                    await tx.RollbackAsync(ct);
                    logSeeder.LogError($"Aborted Seeding — Seeder [{seeder.Name}] : {result.errorMsg}");
                    continue;
                }
                else
                {
                    await tx.CommitAsync(ct);
                    logSeeder.LogInformation($"Seeding finished — [{seeder.Name}] : Success={result.ok}");
                }
            }
            // это не ошибка, а сигнал об отмене операции
            catch (OperationCanceledException)
            {
                await tx.RollbackAsync(); // аккуратно откатываем, если транзакция была открыта
                throw;                    // пробрасываем дальше, чтобы отмена дошла до Host
            }
            // а в это уже обычная Unhandled ошибка
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                logSeeder.LogError(ex, "Unhandled exception in seeder {SeederName}", seeder.Name);
            }
        }
    }
}