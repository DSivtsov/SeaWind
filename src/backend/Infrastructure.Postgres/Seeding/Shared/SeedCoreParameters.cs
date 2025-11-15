using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres.Seeding.Shared
{
    
    public sealed class SeedCoreParameters
    {
        public string FileNameDataTemplate { get; init; } = "Demo{Entity}.json";

        public string FileNameSeedTemplate { get; init; } = "Demo{Entity}.seed.json";

        public string SuffixSeederClassName { get; init; } = "Seeder";
    }

    internal interface ISeedCoreOptions
    {
        SeedCoreParameters Get();
    }

    internal sealed class DefaultSeedCoreOptions : ISeedCoreOptions
    {
        private readonly SeedCoreParameters _opt;

        public DefaultSeedCoreOptions(IOptions<SeedCoreParameters> opt)
        {
            _opt = opt.Value;
        }

        public SeedCoreParameters Get() => _opt;
    }
}
