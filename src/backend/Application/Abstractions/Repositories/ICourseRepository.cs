using Application.DtoCourse;
using Application.Models;

namespace Application.Abstractions.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAlCoursesAsync();
    
    Task<IEnumerable<Lecture>> GetAllLecturesByCourseIdOrderedAscAsyn(string courseId);

    Task<IEnumerable<Exercise>> GetAllExercisesByCourseIdOrderedAscAsyn(string courseId);

    Task<Course?> GetCourseByIdAsync(string courseId);
    Task<List<ExercisesListItemWithMarkDto>> GetExercisesAscWithMarkAsync(string courseId, string studentId);
}
