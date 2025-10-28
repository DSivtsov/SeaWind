using Application.Models;

namespace Application.Abstractions.Repositories;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();
}
