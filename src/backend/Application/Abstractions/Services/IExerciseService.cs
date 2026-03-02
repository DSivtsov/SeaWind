using Application.Dto.Exercise;
using Application.DtoCourse;

namespace Application.Abstractions.Services;

public interface IExerciseService
{
    Task<ExerciseDto?> GetExerciseByIdAsync(Guid exerciseId);
    Task<ExerciseContentDto?> GetExerciseContentByExerciseId(Guid exerciseId);
}
