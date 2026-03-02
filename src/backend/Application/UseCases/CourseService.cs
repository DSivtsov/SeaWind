using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DtoCourse;
using Application.Models;

namespace Application.UseCases;

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

    public async Task<IEnumerable<ExercisesListItemDto>> GetAllExercisesByCourseIdOrderedAsync(string id)
    {
        IEnumerable<Exercise> exercises = await _courseRepository.GetAllExercisesByCourseIdOrderedAscAsyn(id);

        return exercises.Select(lec => new ExercisesListItemDto(lec.Id, lec.OrderNo, lec.Title, lec.ShortDescription));
    }

    public async Task<IEnumerable<LectureListItemDto>> GetAllLecturesByCourseIdOrderedAsync(string id)
    {
        IEnumerable<Lecture> lectures = await _courseRepository.GetAllLecturesByCourseIdOrderedAscAsyn(id);

        return lectures.Select(lec => new LectureListItemDto(lec.Id, lec.OrderNo, lec.Title,
            lec.VideoUrl, lec.Description));
    }

    public async Task<CourseDto?> GetCourseByIdAsync(string courseId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);

        return course == null ? null : new CourseDto(course.Id, course.Title, course.Description);
    }
}
