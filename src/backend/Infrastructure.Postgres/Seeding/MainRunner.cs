using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres.Seeding;

public interface IMainRunnerSeeding
{
    Task Run(CancellationToken ct = default);
}

// класс для запуска сидирования
internal sealed class MainRunner : IMainRunnerSeeding
{
    private readonly IEnumerable<IDbContextRunner> _runners;
    private readonly IOptions<SeedCoreParameters> _coreOpt;
    private readonly ILogger<MainRunner> _logger;
    private readonly SeedPresetAnalyzer _seedPresetAnalyzer;


    public MainRunner(IEnumerable<IDbContextRunner> runners,
                    IOptions<SeedCoreParameters> coreOpt,
                    ILogger<MainRunner> logger,
                    SeedPresetAnalyzer seedPresetAnalyzer)
    {
        _runners = runners;
        _coreOpt = coreOpt;
        _logger = logger;
        _seedPresetAnalyzer = seedPresetAnalyzer;
    }

    public async Task Run(CancellationToken ct = default)
    {
        if (_seedPresetAnalyzer.GetAndCheckIsSeedOff())
        {
            _logger.LogInformation("[SEEDING OFF]");
            return;
        }

        _logger.LogInformation("[SEEDING START]");

        SeedCoreParameters coreValues;
        try { coreValues = _coreOpt.Value;}
        catch (OptionsValidationException ex)
        {
            _logger.LogCritical(ex, "Invalid [Seed:CoreOption]. Aborting seeding : {Error}", ex.Message);
            return;
        }

        _logger.LogInformation("Format DemoData files [{FileNameDataTemplate}] [{FileNameSeedTemplate}] [{SuffixSeederClassName}]",
            coreValues.FileNameDataTemplate, coreValues.FileNameSeedTemplate, coreValues.SuffixSeederClassName);

        foreach (IDbContextRunner runner in _runners)
        {
            await runner.RunAsync(ct);
        }

        _logger.LogInformation("[SEEDING FINISHED]");
    }
}