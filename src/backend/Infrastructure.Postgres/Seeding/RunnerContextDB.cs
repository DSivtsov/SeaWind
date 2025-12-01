using Infrastructure.Postgres.Seeding.ContextDB;
using Infrastructure.Postgres.Seeding.SeedDataFiles;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding;

internal interface IDbContextRunner
{
    Task<bool> RunAsync(CancellationToken ct = default);
}

internal sealed class RunnerContextDB<TContext> : IDbContextRunner where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logSeeder;
    private readonly IConfiguration _cfg;
    private readonly string _prefix;

    private bool IsEnabledDbContextSeeding() => _cfg.GetValue(_prefix + "Enabled", false);

    public RunnerContextDB(IServiceProvider serviceProvider, IConfiguration cfg, ILoggerFactory lf)
    {
        _serviceProvider = serviceProvider;
        _cfg = cfg;
        string nameDbContext = typeof(TContext).Name;
        _prefix = $"Seed:{nameDbContext}:";
        _logSeeder = lf.CreateLogger($"Seeding{nameDbContext}");
    }

    public async Task<bool> RunAsync(CancellationToken ct = default)
    {
        _logSeeder.LogInformation("Start Seeding [{DbContext}]...", typeof(TContext).Name);

        if (!IsEnabledDbContextSeeding())
        {
            _logSeeder.LogInformation("Not Enabled Seeding [{DbContext}]", typeof(TContext).Name);
            return false;
        }

        var checkerOption = new Options<TContext>(_logSeeder, _cfg, _prefix);
        var optRez = checkerOption.CheckAndGet();
        if (!optRez.Ok)
        {
            _logSeeder.LogError("Abort Seeding. Options failed: {Error}", optRez.Error);
            return false;
        }

        var seedPresetAnalyzer = _serviceProvider.GetRequiredService<SeedPresetAnalyzer>(); ;
        optRez = seedPresetAnalyzer.ApplySeedPreset(optRez);

        var runnerSeedDataFiles = new RunnerSeedDataFiles(_logSeeder);
        var rezOk = runnerSeedDataFiles.Run(optRez.PathBase, optRez.UUIDmode);
        if (!rezOk)
        {
            _logSeeder.LogError("Abort Seeding. Error in DataFiles.");
            return false;
        }

        var dbContextChecker = ActivatorUtilities.CreateInstance<Checker<TContext>>(_serviceProvider, _logSeeder);
        bool dbContextRez;
        try
        {
            dbContextRez = await dbContextChecker.Check(ct);
        }
        catch (PendingMigrationsException ex)
        {
            if (optRez.AutoMigrate)
            {
                var migrator = ActivatorUtilities.CreateInstance<Migrate<TContext>>(_serviceProvider, _logSeeder);
                await migrator.Run(ct);
                dbContextRez = true;
            }
            else
            {
                _logSeeder.LogError("DbContext check failed: {Error}", ex.Message);
                dbContextRez = false;
            }
        }

        if (!dbContextRez)
        {
            _logSeeder.LogError("Abort Seeding. DbContext not Ready.");
            return false;
        }

        var seedUUIDStateChecher = new SeedUUIDStateChecker(_logSeeder, optRez, seedPresetAnalyzer.IsOptionsUnderFullManualControl);
        optRez = seedUUIDStateChecher.FreshExistenDataIfNeed();

        if (optRez.ExistenData == ExistenData.Fresh)
        {
            var cleaner = _serviceProvider.GetRequiredService<Cleaner<TContext>>();
            await cleaner.RunCleaningAsync(_logSeeder, ct);
        }

        var seeder = _serviceProvider.GetRequiredService<Seeder<TContext>>();
        await seeder.RunSeedingAsync(optRez, _logSeeder, ct);

        seedUUIDStateChecher.StoreCurrentUsedSeedUUID();

        return true;
    }
}