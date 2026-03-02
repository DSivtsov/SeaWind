namespace Application.Models;

/// <summary>
/// Модель с дополнительной информацией к упражнению.
/// </summary>
public sealed class ExerciseContent
{
    /// <summary>Уникальный идентификатор упражнения из Exercise</summary>
    public Guid ExerciseId { get; set; }

    /// <summary>Подробное описание задания упражнения</summary>
    public string Details { get; set; } = string.Empty;

    // Храним JSON как строку (в БД jsonb). Всегда "[]", не null.
    /// <summary>Набор блоков с дополнительной информацией к упражнению</summary>
    public string ContentBlocksJson { get; set; } = "[]";

    // Обязательный пустой конструктор для EF
    public ExerciseContent() { }

    public ExerciseContent(Guid exerciseId, string details, string contentBlocksJson)
    {
        ExerciseId = exerciseId;
        Details = details;
        ContentBlocksJson = string.IsNullOrWhiteSpace(contentBlocksJson) ? "[]" : contentBlocksJson;
    }
}
