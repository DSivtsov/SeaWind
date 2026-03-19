using Application.Abstractions.Repositories;
using Application.DtoCourse;
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

    public async Task<IEnumerable<Exercise>> GetAllExercisesByCourseIdOrderedAscAsyn(string courseId)
    {
        return await _mainDbContext.Exercises
                .AsNoTracking()
                .Where(lec => lec.CourseId == courseId)
                .OrderBy(lec => lec.OrderNo)
                .ToListAsync();
    }

    public Task<List<ExercisesListItemWithMarkDto>> GetExercisesAscWithMarkAsync(string courseId, string studentId)
    {
        return (from ex in _mainDbContext.Exercises.AsNoTracking()
                join se in _mainDbContext.StudentExercises.AsNoTracking()
                    on ex.Id equals se.ExerciseId into seGroup
                from se in seGroup
                    .Where(x => x.StudentId == studentId)
                    .DefaultIfEmpty()
                where ex.CourseId == courseId
                orderby ex.OrderNo
                select new ExercisesListItemWithMarkDto(ex.Id, ex.OrderNo, ex.Title, ex.ShortDescription,
                    se.Mark != null ? se.Mark : null)
                ).ToListAsync();
    }
}
