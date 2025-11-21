using Infrastructure.Postgres.Seeding.Shared;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

public class SeedUUIDState
{
    public UUIDMode UUIDMode { get; set; } = default!;
    public bool ManualControl { get; set; } = default!; //IsOptionsUnderFullManualControl
    public DateTime LastSeedUtc { get; set; }
}
