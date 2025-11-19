namespace Infrastructure.Postgres.Seeding.Shared;

internal interface IPathFile
{
    string GetDataFile(string seederName, string pathBase);
}

internal class PathFile : IPathFile
{
    private readonly string _fileNameDataTemplate;
    private readonly string _suffixSeederClassName;

    public PathFile(ISeedCoreOptions seedCoreOptions)
    {
        var seedCoreParameters = seedCoreOptions.Get();

        _fileNameDataTemplate = seedCoreParameters.FileNameDataTemplate;
        _suffixSeederClassName = seedCoreParameters.SuffixSeederClassName;
    }

    private string GetNameFile(string _seederName)
    {
        if (!_seederName.EndsWith(_suffixSeederClassName, StringComparison.Ordinal))
            throw new InvalidOperationException($"Seeder class name must end with '{_suffixSeederClassName}': {_seederName}");

        var entityName = _seederName[..^_suffixSeederClassName.Length];

        string nameFile = _fileNameDataTemplate.Replace("{Entity}", entityName, StringComparison.Ordinal);

        return nameFile;
    }

    public string GetDataFile(string seederName, string pathBase)
    {
        string nameFile = GetNameFile(seederName);

        var pathDataFile = Path.Combine(pathBase, nameFile);

        if (File.Exists(pathDataFile))
            return pathDataFile;

        throw new FileNotFoundException($"Absent File : [{pathDataFile}]");
    }

    // for support the "use two paths" option - pathBase & pathVersion
    public string GetDataFileVersion(string seederName, string pathBase, string pathVersion)
    {
        string nameFile = GetNameFile(seederName);

        var fileVersion = Path.Combine(pathVersion, nameFile);

        if (File.Exists(fileVersion))
            return fileVersion;

        var fileBase = Path.Combine(pathBase, nameFile);

        if (File.Exists(fileBase))
            return fileBase;

        throw new FileNotFoundException($"Absent Files with Version and Base : [{fileVersion}] [{fileBase}]");
    }
}
