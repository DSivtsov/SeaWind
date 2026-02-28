using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Dto.Exercise;
using Application.DtoCourse;
using Application.Models;

namespace Application.UseCases;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    internal readonly IExerciseContentRepository _exerciseContentRepository;
    public ExerciseService(IExerciseRepository exerciseRepository, IExerciseContentRepository exerciseContentRepository)
    {
        _exerciseRepository = exerciseRepository;
        _exerciseContentRepository = exerciseContentRepository;
    }

    public async Task<ExerciseDto?> GetExerciseByIdAsync(Guid exerciseId)
    {
        var exercise = await _exerciseRepository.GetExerciseByIdAsync(exerciseId);

        return exercise is null ? null : new ExerciseDto(exercise.OrderNo, exercise.Title);
    }

    public async Task<ExerciseContentDto?> GetExerciseContentByExerciseId(Guid exerciseId)
    {
        var entity = await _exerciseContentRepository.GetByExerciseContentByIdAsync(exerciseId);
        if (entity is null) return null;

        var blocks = ExerciseContentBlockSerializer.ReadBlocks(entity);
        return new ExerciseContentDto(entity.Details ?? "", blocks);
    }

}
