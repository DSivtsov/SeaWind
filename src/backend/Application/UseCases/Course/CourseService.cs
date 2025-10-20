using Application.Abstractions.Services;
using Application.Dto.Course;

namespace Application.UseCases.Course;

public class CourseService : ICourseService
{
    public Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        //throw new NotImplementedException();
        // TODO: https://github.com/DSivtsov/SeaWind/issues/67
        return Task.FromResult<IEnumerable<CourseDto>>(Array.Empty<CourseDto>());
    }
}
