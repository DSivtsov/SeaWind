using Application.DtoTime.Car;
using Application.ModelsTime;
using Infrastructure.Postgres.Seeding;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Time.Seeders;

internal sealed class CarsSeeder : BaseSeeder<TimeDbContext, Car, CarDto>
{
    public override SeedOrder Order => SeedOrder.Core;

    // Ищем по уникальному ключу (Id) — быстрее и устойчивее для upsert
    protected override async Task<Car?> TryFindEntity(TimeDbContext db, CarDto recDemo, CancellationToken ct)
    {
        return await db.Cars.FirstOrDefaultAsync(x => x.Id == recDemo.Id, ct);
    }

    // Добавляем запись в БД
    protected override void AddNewEntity(TimeDbContext dbContext, CarDto recDemo)
        => dbContext.Cars.Add(new Car(recDemo.Id, recDemo.Model, recDemo.Owner, recDemo.RegNumber));

    // обновляем только то, что разрешено
    protected override void UpdateEntity(Car entity, CarDto recDemo)
    {
        entity.Model = recDemo.Model;
        entity.Owner = recDemo.Owner;
        entity.RegNumber = recDemo.RegNumber;
    }

    protected override async Task RemoveEntities(TimeDbContext db, CancellationToken ct)
    {
        var dbSet = db.Cars;
        dbSet.RemoveRange(await dbSet.ToListAsync(ct));
    }

}

