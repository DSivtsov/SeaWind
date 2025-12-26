namespace Application.DtoCourse;

/// <summary>
/// CourseDto транспортный объект (data transfer)
/// </summary>
/// <param name="Id">Id курса</param>
/// <param name="Title">Название курса</param>
/// <param name="Description">Описание курса</param>
public record CourseDto(string Id, string Title, string? Description);
