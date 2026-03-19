namespace Application.DtoCourse;

/// <summary>
/// ExercisesListItemDto для GetAllExercisesByCourseIdAsync
/// </summary>
public record ExercisesListItemWithMarkDto(Guid Id, int OrderNo, string Title, string? ShortDescription, int? Mark);
