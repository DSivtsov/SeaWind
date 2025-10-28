using Application.Abstractions.Repositories;
using Application.Models;

namespace Infrastructure.Postgres.Main.Repositories;

public class CourseRepositoryPostgres : ICourseRepository
{
    public Task<IEnumerable<Course>> GetAllAsync()
    {
        IEnumerable<Course> demo = new[]
        {
            new Course(Guid.NewGuid(), "C# Basics", "CS101", "Intro to C#", null, null),
            new Course(Guid.NewGuid(), "Unity Intro", "UN201", "GameDev basics", null, null)
        };

        return Task.FromResult(demo);
    }
}
