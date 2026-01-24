using Application.DtoCourse;

namespace Application.Abstractions.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();

    Task<CourseDto?> GetCourseByIdAsync(string id);

    Task<IEnumerable<LectureListItemDto>> GetAllLecturesByCourseIdAsync(string id);
}
