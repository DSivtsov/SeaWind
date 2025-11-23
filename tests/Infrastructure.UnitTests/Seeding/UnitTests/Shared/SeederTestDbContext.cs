using System.Text.Json;

namespace Infrastructure.UnitTests.Seeding.UnitTests.Shared;

internal static class SeederTestDbContext
{
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
