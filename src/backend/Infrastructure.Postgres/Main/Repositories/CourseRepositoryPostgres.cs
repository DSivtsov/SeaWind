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

    public async Task<IEnumerable<Course>> GetAlCoursesAsync()
    {
        return await _mainDbContext.Courses.AsNoTracking().ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(string courseId)
    {
        return await _mainDbContext.Courses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == courseId);
    }

    public async Task<IEnumerable<Lecture>> GetAllLecturesByCourseIdOrderedAscAsyn(string courseId)
    {
        return await _mainDbContext.Lectures
                        .AsNoTracking()
                        .Where(lec => lec.CourseId == courseId)
                        .OrderBy(lec => lec.OrderNo)
                        .ToListAsync();
    }
}
