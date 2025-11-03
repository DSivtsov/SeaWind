using Application.DtoMain.Course;
using Application.Models;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Seeders
{
    internal class CourseSeeder : BaseSeeder<MainDbContext, Course, CourseDto>
    {
        public override SeedOrder Order => SeedOrder.Base;

        protected override void AddNewEntity(MainDbContext dbContext, CourseDto recDemo)
        {
            dbContext.Courses.Add(new Course(recDemo.Id, recDemo.Title, recDemo.Code, recDemo.Description));
        }

        protected override async Task RemoveEntities(MainDbContext dbContext, CancellationToken ct)
        {
            var dbSet = dbContext.Courses;
            dbSet.RemoveRange(await dbSet.ToListAsync(ct));
        }

        protected override async Task<Course?> TryFindEntity(MainDbContext dbContext, CourseDto recDemo, CancellationToken ct)
        {
            return await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == recDemo.Id, ct);
        }

        protected override void UpdateEntity(Course entity, CourseDto recDemo)
        {
            entity.Title = recDemo.Title;
            entity.Code = recDemo.Code;
            entity.Description = recDemo.Description;
        }
    }
}