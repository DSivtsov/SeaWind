using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DtoCourse;
using Application.Models;

namespace Application.UseCasesCourse;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        IEnumerable<Course> courses = await _courseRepository.GetAllAsync();

        return courses.Select(c => new CourseDto(c.Id, c.Title, c.Code, c.Description));
    }
}
