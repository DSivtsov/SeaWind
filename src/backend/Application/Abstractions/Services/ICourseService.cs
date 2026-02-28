using Application.DtoCourse;

namespace Application.Abstractions.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();

    Task<CourseDto?> GetCourseByIdAsync(string courseId);

    Task<IEnumerable<LectureListItemDto>> GetAllLecturesByCourseIdOrderedAsync(string courseId);

    Task<IEnumerable<ExercisesListItemDto>> GetAllExercisesByCourseIdOrderedAsync(string courseId);
}
