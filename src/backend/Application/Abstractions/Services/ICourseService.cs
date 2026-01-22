using Application.DtoCourse;

namespace Application.Abstractions.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();

    Task<CourseDto?> GetByIdAsync(string id);
}
