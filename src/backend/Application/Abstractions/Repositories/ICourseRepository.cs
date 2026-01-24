using Application.DtoCourse;
using Application.Models;

namespace Application.Abstractions.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAlCoursesAsync();
    
    Task<IEnumerable<Lecture>> GetAllLecturesByCourseIdOrderedAscAsyn(string id);

    Task<Course?> GetCourseByIdAsync(string courseId);
}
