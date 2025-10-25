using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Dto.Course;

namespace Application.UseCases.Course;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        IEnumerable<Models.Course> courses = await _courseRepository.GetAllAsync();

        return courses.Select(c => new CourseDto(c.Id, c.Title, c.Code, c.Description));
    }
}
