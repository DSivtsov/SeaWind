namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal sealed class SeedFilesLocator
{
    private const string SEED_TEMPLATE = "*.seed.json";
    private readonly string _pathBase;

    internal SeedFilesLocator(string pathBase)
    {
        _pathBase = pathBase;
    }

    internal IEnumerable<string> LocateFiles()
    {
        if (!Directory.Exists(_pathBase))
            throw new InvalidDataException($"Directory for seed files not found [{_pathBase}]");

        IEnumerable<string> seedFilePaths = Directory.EnumerateFiles(_pathBase, SEED_TEMPLATE)
            .OrderBy(fileName => fileName, StringComparer.Ordinal)
            .Select(x => x);

        if (!seedFilePaths.Any())
            throw new InvalidDataException($"No seed files in directory [{_pathBase}]");

        return seedFilePaths;
    }
}
