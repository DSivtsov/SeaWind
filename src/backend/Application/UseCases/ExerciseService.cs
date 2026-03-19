using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Dto.Exercise;
using Application.DtoCourse;

namespace Application.UseCases;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IExerciseContentRepository _exerciseContentRepository;

    public ExerciseService(IExerciseRepository exerciseRepository, IExerciseContentRepository exerciseContentRepository)
    {
        _exerciseRepository = exerciseRepository;
        _exerciseContentRepository = exerciseContentRepository;
    }

    public async Task<ExerciseDto?> GetExerciseAsync(Guid exerciseId)
    {
        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        return exercise is null ? null : new ExerciseDto(exercise.OrderNo, exercise.Title);
    }

    public async Task<ExerciseContentDto?> GetExerciseContentAsync(Guid exerciseId)
    {
        var entity = await _exerciseContentRepository.GetByExerciseContentByIdAsync(exerciseId);
        if (entity is null) return null;

        var blocks = entity.ReadBlocks();
        return new ExerciseContentDto(entity.Details ?? "", blocks);
    }
}
