using Infrastructure.Postgres.Seeding.Shared;

namespace Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders.Shared;

internal sealed class TestSeedCoreOptions : ISeedCoreOptions
{
    public SeedCoreParameters Get() => new SeedCoreParameters();
}
