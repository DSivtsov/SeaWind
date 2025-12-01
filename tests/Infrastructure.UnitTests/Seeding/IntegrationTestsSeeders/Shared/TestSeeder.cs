using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Seeding.IntegrationTestsSeeders.Shared;

internal class TestSeeder : BaseSeeder<TestDbContext, TestEntity, TestSeederDto>
{
    public override SeedOrder Order => SeedOrder.Base;

    protected override void AddNewEntity(TestDbContext dbContext, TestSeederDto recDemo)
    {
        dbContext.TestTable.Add(new TestEntity(recDemo.Id, recDemo.Value));
    }

    protected async override Task RemoveEntities(TestDbContext dbContext, CancellationToken ct)
    {
        var dbSet = dbContext.TestTable;
        dbSet.RemoveRange(await dbSet.ToListAsync(ct));
    }

    protected async override Task<TestEntity?> TryFindEntity(TestDbContext dbContext, TestSeederDto recDemo, CancellationToken ct)
    {
        return await dbContext.TestTable.FirstOrDefaultAsync(x => x.Id == recDemo.Id, ct);
    }

    protected override void UpdateEntity(TestEntity entity, TestSeederDto recDemo)
    {
        entity.Value = recDemo.Value;
    }
}