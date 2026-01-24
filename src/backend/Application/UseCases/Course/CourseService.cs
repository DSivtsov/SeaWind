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
    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        IEnumerable<Course> courses = await _courseRepository.GetAlCoursesAsync();

        return courses.Select(c => new CourseDto(c.Id, c.Title,  c.Description));
    }

    public async Task<IEnumerable<LectureListItemDto>> GetAllLecturesByCourseIdAsync(string id)
    {
        IEnumerable<Lecture> lectures = await _courseRepository.GetAllLecturesByCourseIdOrderedAscAsyn(id);

        return lectures.Select(lec => new LectureListItemDto(lec.Id, lec.CourseId, lec.OrderNo, lec.Title,
            lec.VideoUrl, lec.Description));
    }

    public async Task<CourseDto?> GetCourseByIdAsync(string courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);

        return course == null ? null : new CourseDto(course.Id, course.Title, course.Description);
    }
}
