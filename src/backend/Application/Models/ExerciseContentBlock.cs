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
