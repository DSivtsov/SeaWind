namespace Application.Models;

/// <summary>
/// Модель курса (domain/application level)
/// </summary>
/// <param name="Id">Id курса</param>
/// <param name="Title">Название курса</param>
/// <param name="Code">Код курса</param>
/// <param name="Description">Описание курса</param>
/// <param name="CreatedAt">Дата и время создания курса</param>
/// <param name="UpdatedAt">Дата и время последнего обновления курса</param>
public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; } = null;
    public DateTime? UpdatedAt { get; set; } = null;

    // Обязательный пустой конструктор для EF
    public Course() { }

    // Удобный конструктор для ручного создания сущности
    public Course(Guid id, string title, string? code, string? description)
    {
        Id = id;
        Title = title;
        Code = code;
        Description = description;
    }
    
    public Course(Guid id, string title, string? code, string? description, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Title = title;
        Code = code;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}