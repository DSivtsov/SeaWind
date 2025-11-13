using Application.DtoCourse;
using Application.Models;
using Infrastructure.Postgres.Seeding.Seeders;
using Infrastructure.Postgres.Seeding.Shared;

namespace Infrastructure.Postgres.Main.Seeders
{
    internal class CourseSeeder : BaseSeeder<MainDbContext, Course, CourseDto>
    {
        public override SeedOrder Order => throw new NotImplementedException();

        protected override void AddNewEntity(MainDbContext dbContext, CourseDto recDemo)
        {
            throw new NotImplementedException();
        }

        protected override Task RemoveEntities(MainDbContext dbContext, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        protected override Task<Course?> TryFindEntity(MainDbContext dbContext, CourseDto recDemo, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntity(Course entity, CourseDto recDemo)
        {
            throw new NotImplementedException();
        }
    }
}