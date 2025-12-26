using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

/// <summary>
/// Модель курса (domain/application level)
/// </summary>
/// <param name="Id">Код курса</param>
/// <param name="Title">Название курса</param>
/// <param name="Description">Описание курса</param>
/// <param name="CreatedAt">Дата и время создания курса</param>
/// <param name="UpdatedAt">Дата и время последнего обновления курса</param>
public class Course
{
    public required string Id { get; init; } // PK, immutable в коде (init)
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; } = null;
    public DateTime? UpdatedAt { get; set; } = null;

    // Обязательный пустой конструктор для EF
    public Course() { }

    [SetsRequiredMembers]
    // Удобный конструктор для ручного создания сущности
    public Course(string id, string title, string? description)
    {
        Id = id;
        Title = title;
        Description = description;
    }

    [SetsRequiredMembers]
    public Course(string id, string title, string? description, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
