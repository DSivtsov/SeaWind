using Application.Abstractions.Services;
using Application.Dto.Course;

namespace Application.UseCases.Course;

public class CourseService : ICourseService
{
    public Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        IEnumerable<CourseDto> demo = new[]
        {
            new CourseDto(Guid.NewGuid(), "C# Basics", "Intro to C#"),
            new CourseDto(Guid.NewGuid(), "Unity Intro", "GameDev basics")
        };

        return Task.FromResult(demo);
    }
}
