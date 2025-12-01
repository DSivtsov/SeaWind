using Infrastructure.Postgres.Seeding.Shared;
using Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders;

/// <summary>
/// Набор интеграционных тестов для <see cref="TestSeeder"/>,
/// проверяющий основные режимы работы сидера:
/// baseline-сидирование, InsertOnly (без обновления),
/// вставку отсутствующих записей и обновление существующих.
/// </summary>
public class TestSeederSeedingTests

{
    private readonly ISeeder<TestDbContext> _iTestSeeder;
    private readonly IPathFile _getterpathDemoData;
    private readonly string _pathBase;
    private readonly string _pathDemoDataFile;

    public TestSeederSeedingTests()
    {
        var testSeeder = new TestSeeder();
        _iTestSeeder = testSeeder;

        ISeedCoreOptions seedCoreOptions = new TestSeedCoreOptions();
        _getterpathDemoData = new PathFile(seedCoreOptions);

        _pathBase = Path.Combine(AppContext.BaseDirectory, "Seeding/TestFiles/IntegrationTestsSeeders");

        _pathDemoDataFile = _getterpathDemoData.GetDataFile(_iTestSeeder.Name, _pathBase);
    }

    /// <summary>
    /// Baseline-тест сидера: проверяет, что при пустой БД
    /// <see cref="TestSeeder"/> корректно загружает демо-данные
    /// и вставляет все записи в режиме <c>InsertOnly</c>.
    /// </summary>
    [Fact]
    public async Task TestSeeder_AddNewEntity_WorkAsExpected()
    {
        // Arrange
        using var db = new TestDbContext();
        var result = await _iTestSeeder.LoadAndValidateAsync(_pathDemoDataFile);

        var (ok, errorMsg) = await _iTestSeeder.SeedAsync(db, SeedInsertMode.InsertOnly);

        // Act — создаём снимок состояния БД
        var snapshot = await db.TestTable
            .OrderBy(x => x.Id)
            .Select(x => new
            {
                Id = $"[{x.Id}]",
                x.Value
            })
            .ToListAsync();

        // Assert — Verify создаёт / проверяет verified.json
        await Verify(snapshot);
    }

    /// <summary>
    /// Тест режима <c>InsertOnly</c>: проверяет, что сидер
    /// не обновляет уже существующие записи в БД,
    /// даже если значения отличаются от демо-данных.
    /// </summary>
    [Fact]
    public async Task TestSeeder_NotUpdateExistedEntity_WorkAsExpected()
    {
        // Arrange
        using var db = new TestDbContext();
        await db.SeedUpdatedDd(_pathDemoDataFile);

        var result = await _iTestSeeder.LoadAndValidateAsync(_pathDemoDataFile);

        var (ok, errorMsg) = await _iTestSeeder.SeedAsync(db, SeedInsertMode.InsertOnly);


        // Act — создаём снимок состояния БД
        var snapshot = await db.TestTable
            .OrderBy(x => x.Id)
            .Select(x => new
            {
                Id = $"[{x.Id}]",
                x.Value
            })
            .ToListAsync();

        // Assert — Verify создаёт / проверяет verified.json
        await Verify(snapshot);

    }

    /// <summary>
    /// Тест восстановления отсутствующих записей: проверяет,
    /// что сидер в режиме <c>InsertOnly</c> корректно вставляет
    /// те записи из демо-набора, которых нет в текущей БД.
    /// </summary>
    [Fact]
    public async Task TestSeeder_InsertAbsentEntity_WorkAsExpected()
    {
        // Arrange
        using var db = new TestDbContext();
        await db.SeedUpdatedDd(_pathDemoDataFile);

        TestEntity removedEntity = db.TestTable.First();

        db.TestTable.Remove(removedEntity);

        await db.SaveChangesAsync();

        int countAfterRemove = db.TestTable.Count();

        var result = await _iTestSeeder.LoadAndValidateAsync(_pathDemoDataFile);

        var (ok, errorMsg) = await _iTestSeeder.SeedAsync(db, SeedInsertMode.InsertOnly);

        await db.SaveChangesAsync();

        int countAfterInsert = db.TestTable.Count();

        // Act — создаём снимок состояния БД
        var snapshot = await db.TestTable
            .OrderBy(x => x.Id)
            .Select(x => new
            {
                Id = $"[{x.Id}]",
                x.Value
            })
            .ToListAsync();
        
        Assert.Equal(3, countAfterRemove);
        Assert.Equal(4, countAfterInsert);

        // Assert — Verify создаёт / проверяет verified.json
        await Verify(snapshot);

    }

    /// <summary>
    /// Тест режима <c>InsertOrUpdate</c>: проверяет, что сидер
    /// корректно обновляет существующие записи при совпадении ключей,
    /// используя предоставленные демо-данные.
    /// </summary>
    [Fact]
    public async Task TestSeeder_UpdateExistedEntity_WorkAsExpected()
    {
        // Arrange
        using var db = new TestDbContext();
        await db.SeedUpdatedDd(_pathDemoDataFile);

        var result = await _iTestSeeder.LoadAndValidateAsync(_pathDemoDataFile);

        var (ok, errorMsg) = await _iTestSeeder.SeedAsync(db, SeedInsertMode.InsertOrUpdate);


        // Act — создаём снимок состояния БД
        var snapshot = await db.TestTable
            .OrderBy(x => x.Id)
            .Select(x => new
            {
                Id = $"[{x.Id}]",
                x.Value
            })
            .ToListAsync();

        // Assert — Verify создаёт / проверяет verified.json
        await Verify(snapshot);

    }

}
