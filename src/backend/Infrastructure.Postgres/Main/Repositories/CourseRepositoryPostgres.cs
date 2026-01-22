using Application.Abstractions.Repositories;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Main.Repositories;

public class CourseRepositoryPostgres : ICourseRepository
{
    private readonly MainDbContext _mainDbContext;

    public CourseRepositoryPostgres(MainDbContext mainDbContext)
    {
        _mainDbContext = mainDbContext;
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        var result = await _mainDbContext.Courses.AsNoTracking().ToListAsync();

        return result ?? [];
    }

    public async Task<Course?> GetByIdAsync(string courseId)
    {
        var result = await _mainDbContext.Courses.AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == courseId);

        return result;
    }
}
