using Application.Models;

namespace Application.Dto.Exercise;

public sealed record ExerciseChatDto(StudentExerciseDto Exercise,ThreadLocksDto ThreadLocks);

