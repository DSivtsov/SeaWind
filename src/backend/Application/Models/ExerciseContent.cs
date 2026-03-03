using Application.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KindExerciseContentBlock { Picture, Code }

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TypeExerciseContentBlock { CSharp, Json, Text }

/// <summary>
/// Структурный блок, входящий в состав контента упражнения (ExerciseContent).
/// </summary>
public sealed record ExerciseContentBlock(KindExerciseContentBlock Kind, string UrlFile,
    TypeExerciseContentBlock? TypeContent);

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

    public List<ExerciseContentBlock> ReadBlocks()
    {
        var json = ContentBlocksJson;
        if (string.IsNullOrWhiteSpace(json))
            return new List<ExerciseContentBlock>();

        return JsonSerializer.Deserialize<List<ExerciseContentBlock>>(json, AppJson.SerializerOpt)
               ?? new List<ExerciseContentBlock>();
    }

    public ExerciseContent WriteBlocks(List<ExerciseContentBlock> blocks)
    {
        string contentBlocksJson =
            JsonSerializer.Serialize(blocks ?? new List<ExerciseContentBlock>(), AppJson.SerializerOpt);

        return new ExerciseContent(ExerciseId, Details, contentBlocksJson);
    }
}
