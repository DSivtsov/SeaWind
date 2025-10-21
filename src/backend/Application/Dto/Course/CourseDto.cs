namespace Application.Dto.Course;

/// <summary>
/// CourseDto транспортный объект (data transfer)
/// </summary>
/// <param name="Id">Id курса</param>
/// <param name="Title">Название курса</param>
/// <param name="Code">Код курса</param>
/// <param name="Description">Описание курса</param>
/// <param name="CreatedAt">Дата и время создания курса</param>
/// <param name="UpdatedAt">Дата и время последнего обновления курса</param>
public record CourseDto(Guid Id, string Title, string? Code, string? Description, DateTime? CreatedAt, DateTime? UpdatedAt);
