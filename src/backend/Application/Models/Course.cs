namespace Application.Models;

/// <summary>
/// Модель курса (domain/application level)
/// </summary>
/// <param name="Id">Id курса</param>
/// <param name="Title">Название курса</param>
/// <param name="Description">Описание курса</param>
public record Course(Guid Id, string Title, string Description);