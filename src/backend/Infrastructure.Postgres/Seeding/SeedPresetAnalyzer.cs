using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Postgres.Seeding;

internal sealed class SeedPresetAnalyzer
{
    private const string _prefixSeedMode = "Seed:Preset";

    private readonly ILogger _log;
    private readonly IConfiguration _cfg;

    private SeedPreset _seedPreset = SeedPreset.SeedCustom;

    public bool UseEmptyDB => _seedPreset == SeedPreset.EmptyDb;

    public SeedPresetAnalyzer(ILogger<SeedPresetAnalyzer> log, IConfiguration cfg)
    {
        _log = log;
        _cfg = cfg;
    }

    internal OptionsResult ApplySeedPreset(OptionsResult optRez)
    {
        if (IsOptionsUnderFullManualControl)
            return optRez;

        return _seedPreset switch
        {
            SeedPreset.SuperFast => optRez with
            {
                UUIDmode = UUIDMode.Stable,
                ExistenData = ExistenData.NotDel,
                InsertMode = SeedInsertMode.InsertOnly,
                AutoMigrate = true
            },

            SeedPreset.Fast => optRez with
            {
                UUIDmode = UUIDMode.Stable,
                ExistenData = ExistenData.NotDel,
                InsertMode = SeedInsertMode.InsertOrUpdate,
                AutoMigrate = true
            },

            SeedPreset.Real => optRez with
            {
                UUIDmode = UUIDMode.Real,
                ExistenData = ExistenData.Fresh,
                InsertMode = SeedInsertMode.InsertOnly,
                AutoMigrate = true
            },

            // SeedPreset.SeedOFF & .UseEmptyDB должно обрабатываться до вызова ApplySeedPreset(...)
            _ => throw new NotImplementedException($"Not support [{_seedPreset}] preset or error in logic")
        };
    }

    internal bool IsOptionsUnderFullManualControl => _seedPreset == SeedPreset.SeedCustom;

    internal bool GetAndCheckIsSeedOff()
    {
        _seedPreset = GetSeedPreset();

        _log.LogInformation("Seeding Preset={seedPreset}", _seedPreset);

        return _seedPreset == SeedPreset.SeedOFF;
    }

    private SeedPreset GetSeedPreset()
    {
        var seedPresetText = _cfg[_prefixSeedMode];

        if (!Enum.TryParse(seedPresetText, ignoreCase: true, out SeedPreset seedPreset))
            return SeedPreset.SeedCustom;

        return seedPreset;
    }
}
