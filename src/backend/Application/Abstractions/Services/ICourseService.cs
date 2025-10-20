using Application.Dto.Course;

namespace Application.Abstractions.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();
}
