using Application.Models;

namespace Application.Abstractions.Repositories
{
    public interface IExerciseContentRepository
    {
        Task<ExerciseContent?>  GetByExerciseContentByIdAsync(Guid exerciseId);
    }
}
