using Application.Common.Enums;

namespace Application.Dto.Exercise;

public sealed record StudentExerciseDto(StatusStudentExercise Status, int? Mark, string ThreadId);

