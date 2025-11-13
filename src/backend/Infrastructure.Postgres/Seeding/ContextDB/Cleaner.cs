using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding.ContextDB;

internal sealed class Cleaner<TContext> where TContext : DbContext
{
    private readonly IEnumerable<ISeeder<TContext>> _cleanersInReversOrder;
    private readonly TContext _dbContext;

    public Cleaner(IServiceProvider sp, IEnumerable<ISeeder<TContext>> seeders)
    {
        _cleanersInReversOrder = seeders.OrderByDescending(s => s.Order);
        _dbContext = sp.GetRequiredService<TContext>();
    }

    internal async Task RunCleaningAsync(ILogger logSeeder, CancellationToken ct = default)
    {
        foreach (ISeeder<TContext> cleaner in _cleanersInReversOrder)
        {
            logSeeder.LogInformation("Run Cleaner [{Name}]", cleaner.Name);

            await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var result = await cleaner.RemoveRecordsAsync(_dbContext, ct);

                if (!result.ok)
                {
                    await tx.RollbackAsync(ct);
                    logSeeder.LogError($"Aborted Cleaning — Cleaner [{cleaner.Name}] : {result.errorMsg}");
                    continue;
                }
                else
                {
                    await tx.CommitAsync(ct);
                    logSeeder.LogInformation("Cleaning completed for {CleanerName}: Success={Ok}", cleaner.Name, result.ok);

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
                logSeeder.LogError(ex, "Unhandled exception in cleaner {CleanerName}", cleaner.Name);
            }
        }
    }
}