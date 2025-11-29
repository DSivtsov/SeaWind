using Infrastructure.Postgres.Seeding.SeedDataFiles;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.UnitTests.Seeding.ApprovalTests;

[CollectionDefinition("Snapshot_DataFiles", DisableParallelization = true)]
public class SnapshotDataFilesCollection
{
}

/// <summary>
/// Набор интеграционных approval-тестов, проверяющий корректность подготовки demo-данных:
/// целостность входных seed-файлов и детерминированность выходных DataFiles,
/// генерируемых <see cref="RunnerSeedDataFiles"/>.
/// </summary>
[Collection("Snapshot_DataFiles")]
public class DataFiles_Prepare_Tests
{
    private readonly IEnumerable<string> _seedFiles;
    private readonly string _testDataDir; // папка с Demo*.seed.json 
    private readonly IEnumerable<string> _newDatafiles;

    public DataFiles_Prepare_Tests()
    {
        _testDataDir = Path.Combine(AppContext.BaseDirectory, "Seeding/TestFiles/SeedingTests");
        _seedFiles = Directory.EnumerateFiles(_testDataDir, "Demo*.seed.json");

        _newDatafiles = PrepareDataFiles();
    }

    [Fact]
    public async Task Input_Manifest_SeedFiles_is_approved()
    {
        var files = _seedFiles
            .Select(Path.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var manifest = new
        {
            Count = files.Length,
            Files = files
        };

        await Verify(manifest)
            .UseFileName("SeedFiles_manifest_is_approved");
    }

    [Fact]
    public async Task Input_SeedFiles_contents_is_approved()
    {
        var missingFiles = _seedFiles.Where(path => !File.Exists(path)).ToArray();
        if (missingFiles.Length > 0)
        {
            await Verify(missingFiles)
                .UseFileName("Snapshot_Create_Tests.Inputs_snapshot_missing_files");
            return;
        }

        foreach (var path in _seedFiles)
        {
            var content = File.ReadAllText(path);
            var settings = new VerifySettings();
            // имя снапшота: Snapshot_Create_Tests.Inputs_snapshot_is_approved.<FileName>.verified.json
            settings.UseFileName($"Snapshot_Create_Tests.Inputs_snapshot_is_approved.{Path.GetFileName(path)}");
            await Verify(content, settings);
        }
    }

    [Fact]
    public async Task Output_Manifest_DataFiles_is_approved()
    {
        var outputDataFiles = _newDatafiles
            .Select(Path.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var manifest = new
        {
            Count = outputDataFiles.Length,
            Files = outputDataFiles
        };

        await Verify(manifest)
            .UseFileName("Output_DataFiles_is_approved");
    }


    [Fact]
    public async Task Output_DataFiles_contents_is_approved()
    {
        Assert.NotEmpty(_newDatafiles);

        foreach (var path in _newDatafiles)
        {
            var content = File.ReadAllText(path);
            var settings = new VerifySettings();
            // имя снапшота: Snapshot_Create_Tests.Outputs_snapshot_is_approved.<FileName>.verified.json
            settings.UseFileName($"Snapshot_Create_Tests.Outputs_snapshot_is_approved.{Path.GetFileName(path)}");
            await Verify(content, settings);
        }
    }

    private static void DelOldDataFiles(string pathBase)
    {
        foreach (var file in GetOnlyDataFiles(pathBase))
            File.Delete(file);
    }

    private static IEnumerable<string> GetOnlyDataFiles(string pathBase)
        => Directory.EnumerateFiles(pathBase, "Demo*.json")
                    .Where(name => !name.Contains("seed.json"));

    private IEnumerable<string> PrepareDataFiles()
    {
        var logger = NullLogger.Instance;
        var pathBase = _testDataDir;

        DelOldDataFiles(pathBase);

        // use UUIDMode.Stable to receive the determenistic UUID
        var dataFilesPrepare = new RunnerSeedDataFiles(logger);

        dataFilesPrepare.Run(pathBase, UUIDMode.Stable);

        IEnumerable<string> newDatafiles = GetOnlyDataFiles(pathBase);
        return newDatafiles;
    }
}