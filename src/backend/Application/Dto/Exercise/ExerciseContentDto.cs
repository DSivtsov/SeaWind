using Application.Models;

namespace Application.Dto.Exercise;

public sealed record ExerciseContentDto(string Details, List<ExerciseContentBlock> Blocks);
