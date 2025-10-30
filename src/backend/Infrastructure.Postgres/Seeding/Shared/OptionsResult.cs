namespace Infrastructure.Postgres.Seeding.Shared;

internal sealed record OptionsResult(bool Ok, ExistenData ExistenData, SeedMode Mode, string PathBase,
    string PathVersion, bool AutoMigrate, string? Error = null)
    : PhaseResult(Ok, Error);
