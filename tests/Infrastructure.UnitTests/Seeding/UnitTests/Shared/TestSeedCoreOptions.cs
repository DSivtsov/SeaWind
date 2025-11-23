using Infrastructure.Postgres.Seeding.Shared;

namespace Infrastructure.UnitTests.Seeding.UnitTests.Shared;

internal sealed class TestSeedCoreOptions : ISeedCoreOptions
{
    public SeedCoreParameters Get() => new SeedCoreParameters();
}
