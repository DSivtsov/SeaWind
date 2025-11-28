using Infrastructure.Postgres.Seeding.Shared;
using Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders.Shared;

namespace Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders;

public class TestSeederCleaningTests
{
    private readonly ISeeder<TestDbContext> _iTestCleaner;
    private readonly IPathFile _getterpathDemoData;
    private readonly string _pathBase;
    private readonly string _pathDemoDataFile;

    public TestSeederCleaningTests()
    {

        var testCleaner = new TestSeeder();
        _iTestCleaner = testCleaner;

        ISeedCoreOptions seedCoreOptions = new TestSeedCoreOptions();
        _getterpathDemoData = new PathFile(seedCoreOptions);

        _pathBase = Path.Combine(AppContext.BaseDirectory, "Seeding/TestFiles/IntegrationTestsSeeders");

        _pathDemoDataFile = _getterpathDemoData.GetDataFile(_iTestCleaner.Name, _pathBase);
    }

    /// <summary>
    /// Интеграционный тест операции удаления:
    /// проверяет, что <see cref="TestSeeder"/> корректно очищает таблицу
    /// с помощью <c>RemoveRecordsAsync</c>.
    /// </summary>
    [Fact]
    public async Task TestSeeder_RemoveAllEntity_WorkAsExpected()
    {
        using var db = new TestDbContext();
        // Arrange
        await db.SeedOriginalDb(_pathDemoDataFile);

        int countBeforeClean = db.TestTable.Count();

        var (ok, errorMsg) = await _iTestCleaner.RemoveRecordsAsync(db);

        int countAfterClean = db.TestTable.Count();

        Assert.True(ok);
        Assert.Null(errorMsg);

        Assert.Equal(4, countBeforeClean);
        Assert.Equal(0, countAfterClean);
    }
}
