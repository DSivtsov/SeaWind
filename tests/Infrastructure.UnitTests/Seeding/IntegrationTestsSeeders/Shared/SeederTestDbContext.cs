using System.Text.Json;

namespace Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders.Shared;

/// <summary>
/// Вспомогательный класс для интеграционных тестов сидеров: 
/// предоставляет методы расширения для быстрого наполнения <see cref="TestDbContext"/>
/// исходными и модифицированными данными напрямую из демо-файлов,
/// минуя пайплайн сидера.
/// Используется в Arrange-части тестов для формирования контролируемого 
/// состояния БД перед вызовом <c>LoadAndValidateAsync</c> и <c>SeedAsync</c>.
/// </summary>
internal static class SeederTestDbContext
{
    /// <summary>
    /// Заполняет <see cref="TestDbContext"/> исходным набором данных напрямую из demo-файла,
    /// создавая состояние БД, соответствующее первому сидированию.
    /// Используется в Arrange-части тестов для моделирования состояния
    /// «БД после первоначального сидирования», минуя реальный pipeline сидера.
    /// </summary>
    internal static async Task SeedOriginalDb(this TestDbContext db, string path)
    {
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<TestSeederDto>>(json)!;

        foreach (var dto in records)
        {
            db.TestTable.Add(new TestEntity(dto.Id, dto.Value));
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Заполняет <see cref="TestDbContext"/> модифицированным набором данных на основе demo-файла,
    /// создавая состояние БД, отличающееся от входных демо-данных.
    /// Применяется в Arrange-части тестов для проверки поведения сидера при
    /// обновлении существующих записей (update-сценарии).
    /// </summary>
    internal static async Task SeedUpdatedDd(this TestDbContext db, string path)
    {
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<TestSeederDto>>(json)!;

        for (int i = 0; i < records.Count; i++)
        {
            db.TestTable.Add(new TestEntity(records[i].Id, records[i].Value + i.ToString()));
        }

        await db.SaveChangesAsync();
    }
}
