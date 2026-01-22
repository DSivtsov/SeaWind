using Application.Models;
using Infrastructure.Postgres.Main.SeederDto;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Seeders
{
    internal class LectureSeeder : BaseSeeder<MainDbContext, Lecture, LectureSeederDto>
    {
        public override SeedOrder Order => SeedOrder.Core;

        protected override void AddNewEntity(MainDbContext dbContext, LectureSeederDto recDemo)
        {
            dbContext.Lectures.Add(new Lecture(recDemo.Id, recDemo.CourseId, recDemo.OrderNo, recDemo.Title, recDemo.VideoUrl, 
                recDemo.Description));
        }

        protected override async Task RemoveEntities(MainDbContext dbContext, CancellationToken ct)
        {
            var dbSet = dbContext.Lectures;
            dbSet.RemoveRange(await dbSet.ToListAsync(ct));
        }

        protected override async Task<Lecture?> TryFindEntity(MainDbContext dbContext, LectureSeederDto recDemo,
            CancellationToken ct)
        {
            return await dbContext.Lectures.FirstOrDefaultAsync(c => c.Id == recDemo.Id, ct);
        }

        protected override void UpdateEntity(Lecture entity, LectureSeederDto recDemo)
        {
            entity.CourseId = recDemo.CourseId;
            entity.OrderNo = recDemo.OrderNo;
            entity.Title = recDemo.Title;
            entity.VideoUrl = recDemo.VideoUrl;
            entity.Description = recDemo.Description;
        }
    }
}
