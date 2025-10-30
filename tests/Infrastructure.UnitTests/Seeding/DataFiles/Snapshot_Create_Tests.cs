using Infrastructure.Postgres.Seeding.SeedDataFiles;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.UnitTests.Seeding;

public class Snapshot_Create_Tests
{
    private readonly IEnumerable<string> _seedFiles;
    private readonly string _testDataDir; // папка с Demo*.seed.json 

    public Snapshot_Create_Tests()
    {
        _testDataDir = Path.Combine(AppContext.BaseDirectory, "SeedingTestData");
        _seedFiles = Directory.EnumerateFiles(_testDataDir, "Demo*.seed.json");
    }

    [Fact]
    public async Task SeedFiles_manifest_is_approved()
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
    public async Task SeedFiles_snapshot_is_approved()
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
    public async Task DataFiles_snapshot_is_approved()
    {
        //Arrange
        var logger = NullLogger.Instance;
        var pathBase = _testDataDir;
        DelOldDataFiles(pathBase);
        // use UUIDMode.Stable to receive the determenistic UUID
        var dataFilesPrepare = new DataFilesPrepare(logger, pathBase, UUIDMode.Stable);

        //Act
        await dataFilesPrepare.Run();

        //Assert
        IEnumerable<string> newDatafiles = GetOnlyDataFiles(pathBase);
        
        foreach (var path in newDatafiles)
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
}