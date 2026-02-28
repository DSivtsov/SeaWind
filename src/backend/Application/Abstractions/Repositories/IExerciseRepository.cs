using Application.Models;

namespace Application.Abstractions.Repositories;

public interface IExerciseRepository
{
    Task<Exercise?> GetExerciseByIdAsync(Guid exerciseId);
}
