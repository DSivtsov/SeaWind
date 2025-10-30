namespace Infrastructure.Postgres.Seeding.Shared;

internal interface IPathFile
{
    string GetSeedFile(string seederName, string pathBase);

    string GetDataFile(string seederName, string pathBase);
}

internal class PathFile : IPathFile
{
    private readonly string _fileNameDataTemplate;
    private readonly string _fileNameSeedTemplate;
    private readonly string _suffixSeederClassName;

    public PathFile(ISeedCoreOptions seedCoreOptions)
    {
        var seedCoreParameters = seedCoreOptions.Get();

        _fileNameDataTemplate = seedCoreParameters.FileNameDataTemplate;
        _fileNameSeedTemplate = seedCoreParameters.FileNameSeedTemplate;
        _suffixSeederClassName = seedCoreParameters.SuffixSeederClassName;
    }

    private string GetSeedFileName(string entityName) => _fileNameSeedTemplate.Replace("{Entity}", entityName, StringComparison.Ordinal);

    private string GetDataFileName(string entityName) => _fileNameDataTemplate.Replace("{Entity}", entityName, StringComparison.Ordinal);

    private string GetNameSeedFile(string _seederName)
    {
        if (!_seederName.EndsWith(_suffixSeederClassName, StringComparison.Ordinal))
            throw new InvalidOperationException($"Seeder class name must end with '{_suffixSeederClassName}': {_seederName}");

        var entityName = _seederName[..^_suffixSeederClassName.Length];

        return GetSeedFileName(entityName);
    }

    private string GetNameDataFile(string _seederName)
    {
        if (!_seederName.EndsWith(_suffixSeederClassName, StringComparison.Ordinal))
            throw new InvalidOperationException($"Seeder class name must end with '{_suffixSeederClassName}': {_seederName}");

        var entityName = _seederName[..^_suffixSeederClassName.Length];

        return GetDataFileName(entityName);
    }
    public string GetSeedFile(string seederName, string pathBase)
    {
        string nameSeedFile = GetNameSeedFile(seederName);

        var fileBaseSeedFile = Path.Combine(pathBase, nameSeedFile);

        if (File.Exists(fileBaseSeedFile))
            return fileBaseSeedFile;

        throw new FileNotFoundException($"Absent Files with Base SeedFile: [{fileBaseSeedFile}]");
    }

    public string GetDataFile(string seederName, string pathBase)
    {
        string nameDataFile = GetNameDataFile(seederName);

        var fileBaseDataFile = Path.Combine(pathBase, nameDataFile);

        if (File.Exists(fileBaseDataFile))
            return fileBaseDataFile;

        throw new FileNotFoundException($"Absent Files with Base DataFile: [{fileBaseDataFile}]");
    }

    /*    public string Get(string seederName, string pathBase, string pathVersion)
        {
            string nameDemoDataFile = GetNameDemoDataFile(seederName);

            var fileVersion = Path.Combine(pathVersion, nameDemoDataFile);

            if (File.Exists(fileVersion))
                return fileVersion;

            var fileBase = Path.Combine(pathBase, nameDemoDataFile);

            if (File.Exists(fileBase))
                return fileBase;

            throw new FileNotFoundException($"Absent Files with Version and Base DemoData: [{fileVersion}] [{fileBase}]");
        }*/
}
