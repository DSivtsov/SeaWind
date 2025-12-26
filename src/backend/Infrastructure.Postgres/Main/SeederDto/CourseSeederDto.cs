namespace Infrastructure.Postgres.Main.SeederDto;

/// <summary>
/// CourseDto транспортный объект (data transfer)
/// </summary>
/// <param name="Id">Id курса</param>
/// <param name="Title">Название курса</param>
/// <param name="Description">Описание курса</param>
public record CourseSeederDto(string Id, string Title, string? Description);
