namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal sealed class SeedEnvironmentPreparer
{
    private const string SEED_TEMPLATE = "*.seed.json";
    private const string ROOT_FOLDER = "./";

    private readonly string _pathBase;

    internal SeedEnvironmentPreparer(string pathBase)
    {
        _pathBase = pathBase;
    }

    internal IEnumerable<string> PrepareEnvironmentAndLocateFiles()
    {
        try { Directory.SetCurrentDirectory(_pathBase); }
        catch (DirectoryNotFoundException)
        {
            throw new InvalidDataException($"Directory for seed files not found [{_pathBase}]");
        }
        catch (Exception)
        {
            throw new InvalidDataException($"Wrong directory [{_pathBase}]");
        }

        IEnumerable<string> seedFilePaths = Directory.EnumerateFiles(ROOT_FOLDER, SEED_TEMPLATE)
            .OrderBy(fileName => fileName, StringComparer.Ordinal)
            .Select(x => x);

        if (!seedFilePaths.Any())
            throw new InvalidDataException($"No seed files in directory [{_pathBase}]");

        return seedFilePaths;
    }
}
