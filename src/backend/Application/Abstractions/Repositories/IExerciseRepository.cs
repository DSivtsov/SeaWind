using Application.Models;

namespace Application.Abstractions.Repositories;

public interface IExerciseRepository
{
    Task<Exercise?> GetExerciseByIdAsync(Guid exerciseId);

    Task<string> GetCourseIdByExerciseIdAsync(Guid exerciseId, CancellationToken ct);
}
