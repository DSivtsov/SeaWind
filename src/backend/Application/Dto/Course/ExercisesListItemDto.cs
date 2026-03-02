namespace Application.DtoCourse;

/// <summary>
/// ExercisesListItemDto для GetAllExercisesByCourseIdAsync
/// </summary>
public record ExercisesListItemDto(Guid Id, int OrderNo, string Title, string? ShortDescription);
