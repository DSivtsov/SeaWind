using Application.DtoTime.Tester;
using Application.ModelsTime;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Time.Seeders;

internal sealed class TesterSeeder : BaseSeeder<TimeDbContext, Tester, TesterDto>
{
    public override SeedOrder Order => SeedOrder.Base;

    // Ищем по уникальному ключу (Id) — быстрее и устойчивее для upsert
    protected override async Task<Tester?> TryFindEntity(TimeDbContext db, TesterDto recDemo, CancellationToken ct)
    {
        return await db.Testers.FirstOrDefaultAsync(x => x.Id == recDemo.Id, ct);
    }

    // Добавляем запись в БД
    protected override void AddNewEntity(TimeDbContext db, TesterDto recDemo)
        => db.Testers.Add(new Tester(recDemo.Id, recDemo.Name, recDemo.Age));

    // обновляем только то, что разрешено
    protected override void UpdateEntity(Tester entity, TesterDto recDemo)
    {
        entity.Name = recDemo.Name;
        entity.Age = recDemo.Age;
    }

    protected override async Task RemoveEntities(TimeDbContext db, CancellationToken ct)
    {
        var dbSet = db.Testers;
        dbSet.RemoveRange(await dbSet.ToListAsync(ct));
    }
}

