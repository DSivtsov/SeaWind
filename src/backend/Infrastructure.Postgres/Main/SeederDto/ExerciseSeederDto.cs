using Application.Models;

namespace Infrastructure.Postgres.Main.SeederDto;

/// <summary>
/// Exercise Dto для сидирования
/// </summary>
public record ExerciseSeederDto(Guid Id, string CourseId, int OrderNo, string Title, string? ShortDescription);
