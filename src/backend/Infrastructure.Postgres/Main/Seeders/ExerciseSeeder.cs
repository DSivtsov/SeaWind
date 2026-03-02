using Application.Models;
using Infrastructure.Postgres.Main.SeederDto;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Seeders
{
    internal class ExerciseSeeder : BaseSeeder<MainDbContext, Exercise, ExerciseSeederDto>
    {
        public override SeedOrder Order => SeedOrder.Core;

        protected override void AddNewEntity(MainDbContext dbContext, ExerciseSeederDto recDemo)
        {
            dbContext.Exercises.Add(new Exercise(recDemo.Id, recDemo.CourseId, recDemo.OrderNo, recDemo.Title,
                recDemo.ShortDescription));
        }

        protected override async Task RemoveEntities(MainDbContext dbContext, CancellationToken ct)
        {
            var dbSet = dbContext.Exercises;
            dbSet.RemoveRange(await dbSet.ToListAsync(ct));
        }

        protected override async Task<Exercise?> TryFindEntity(MainDbContext dbContext, ExerciseSeederDto recDemo,
            CancellationToken ct)
        {
            return await dbContext.Exercises.FirstOrDefaultAsync(c => c.Id == recDemo.Id, ct);
        }

        protected override void UpdateEntity(Exercise entity, ExerciseSeederDto recDemo)
        {
            entity.CourseId = recDemo.CourseId;
            entity.OrderNo = recDemo.OrderNo;
            entity.Title = recDemo.Title;
            entity.ShortDescription = recDemo.ShortDescription;
        }
    }
}
