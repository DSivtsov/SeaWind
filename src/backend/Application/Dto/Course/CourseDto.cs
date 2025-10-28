namespace Application.DtoCourse;

/// <summary>
/// CourseDto транспортный объект (data transfer)
/// </summary>
/// <param name="Id">Id курса</param>
/// <param name="Title">Название курса</param>
/// <param name="Code">Код курса</param>
/// <param name="Description">Описание курса</param>
public record CourseDto(Guid Id, string Title, string? Code, string? Description);
