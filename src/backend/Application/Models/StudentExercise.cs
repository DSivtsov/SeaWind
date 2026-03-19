using Application.Common.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

/// <summary>
/// Модель состояния выполнения студентом упражнения курса.
/// </summary>
public class StudentExercise
{
    /// <summary>Уникальный идентификатор упражнения</summary>
    public required Guid Id { get; init; } // PK, immutable в коде (init)

    /// <summary>Идентификатор студента, выполняющего данное упражнение</summary>
    public string StudentId { get; set; } = null!;

    /// <summary>Идентификатор упражнение, которое выполняет студент</summary>
    public Guid ExerciseId { get; set; }

    /// <summary>Идентификатор ментора, которое будет отвечать за проверку упражнения</summary>
    public string? AssignedMentorId { get; set; }

    //для упрощение фильтров и сортировок
    /// <summary>Идентификатор курса, к которому относится выполняемое упражнение</summary>
    public string CourseId { get; set; } = null!;

    /// <summary>Статус выполнения студентом упражнения курса</summary>
    public StatusStudentExercise Status { get; set; } = StatusStudentExercise.OnStudent;

    /// <summary>Оценка студента за выполнение упражнение курса</summary>
    public int? Mark { get; set; } = null;

    /// <summary>Дата и время когда студент отправил задание на проверку</summary>
    public DateTime? RequestedCheckAt { get; set; }

    /// <summary>Дата и время когда ментор поставил оценку студенту</summary>
    public DateTime? CheckedAt { get; set; }

    /// <summary>Id thread чата в Монго в рамках которого ведется переписка по упражнению</summary>
    public string ThreadId { get; set; } = null!;

    // Обязательный пустой конструктор для EF
    protected StudentExercise() { }

    [SetsRequiredMembers]
    // Удобный конструктор для ручного создания сущности
    public StudentExercise(string studentId, Guid exerciseId, string courseId, string threadId)
    {
        StudentId = studentId;
        ExerciseId = exerciseId;
        CourseId = courseId;
        ThreadId = threadId;
    }
}
