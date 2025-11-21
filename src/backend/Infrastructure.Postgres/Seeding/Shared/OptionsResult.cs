namespace Infrastructure.Postgres.Seeding.Shared;

internal sealed record OptionsResult(bool Ok, ExistenData ExistenData, SeedInsertMode InsertMode, string PathBase,
    string PathVersion, bool AutoMigrate, UUIDMode UUIDmode, string? Error = null)
    : PhaseResult(Ok, Error);
