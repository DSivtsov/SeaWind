using Application.Dto.Exercise;
using Application.DtoCourse;

namespace Application.Abstractions.Services;

public interface IExerciseService
{
    Task<ExerciseDto?> GetExerciseAsync(Guid exerciseId);
    Task<ExerciseContentDto?> GetExerciseContentAsync(Guid exerciseId);
}
