using Application.Models;

namespace Infrastructure.Postgres.Main.SeederDto;

/// <summary>
/// Lecture Dto для сидирования
/// </summary>
public record LectureSeederDto(Guid Id, string CourseId, int OrderNo, string Title, string? VideoUrl, string? Description);
