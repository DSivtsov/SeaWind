using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

/// <summary>
/// Модель упражнения курса.
/// </summary>
public class Exercise
{
    /// <summary>Уникальный идентификатор упражнения</summary>
    public required Guid Id { get; init; } // PK, immutable в коде (init)

    /// <summary>Идентификатор курса, к которому относится упражнение</summary>
    public string CourseId { get; set; } = null!;

    /// <summary>Порядковый номер упражнения внутри курса</summary>
    public int OrderNo { get; set; }

    /// <summary>Название упражнения</summary>
    public string Title { get; set; } = null!;

    /// <summary>Краткое описание упражнения (необязательное поле)</summary>
    public string? ShortDescription { get; set; }

    // Обязательный пустой конструктор для EF
    protected Exercise() { }

    [SetsRequiredMembers]
    // Удобный конструктор для ручного создания сущности
    public Exercise(Guid id, string courseId, int orderNo, string title, string? shortDescription)
    {
        Id = id;
        CourseId = courseId;
        OrderNo = orderNo;
        Title = title;
        ShortDescription = shortDescription;
    }
}
