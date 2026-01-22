using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

public enum LectureStatus
{
    Published,
    Draft,
}

/// <summary>
/// Модель лекции курса.
/// </summary>
public class Lecture
{
    /// <summary>Уникальный идентификатор лекции.</summary>
    public required Guid Id { get; init; } // PK, immutable в коде (init)

    /// <summary>Идентификатор курса, к которому относится лекция</summary>
    public string CourseId { get; set; } = null!;

    /// <summary>Порядковый номер лекции внутри курса.</summary>
    public int OrderNo { get; set; }

    /// <summary>Название лекции.</summary>
    public string Title { get; set; } = null!;

    /// <summary>URL видео лекции (необязательное поле).</summary>
    public string? VideoUrl { get; set; }

    /// <summary>Описание лекции (необязательное поле).</summary>
    public string? Description { get; set; }

    /// <summary>Статус лекции (Published или Draft).</summary>
    public LectureStatus Status { get; set; } = LectureStatus.Published;

    /// <summary>Дата и время создания лекции.</summary>
    public DateTime? CreatedAt { get; set; } = null;

    /// <summary>Дата и время последнего обновления лекции.</summary>
    public DateTime? UpdatedAt { get; set; } = null;

    // Обязательный пустой конструктор для EF
    public Lecture() { }

    [SetsRequiredMembers]
    // Удобный конструктор для ручного создания сущности
    public Lecture(Guid id, string courseId, int orderNo, string title, string? videoUrl, string? description)
    {
        Id = id;
        CourseId = courseId;
        OrderNo = orderNo;
        Title = title;
        VideoUrl = videoUrl;
        Description = description;
    }

    [SetsRequiredMembers]
    public Lecture(Guid id, string courseId, int orderNo, string title, string? videoUrl, string? description, LectureStatus status, 
        DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        CourseId = courseId;
        OrderNo = orderNo;
        Title = title;
        VideoUrl = videoUrl;
        Description = description;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
