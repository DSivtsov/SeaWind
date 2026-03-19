using Application.Common.Enums;

namespace Application.Dto.ChatExercise;

public sealed record MentorExerciseChatInboxResponse(
    string StudentExerciseId,
    string StudentId,
    string Email,
    Guid ExerciseId,
    string ExerciseTitle,
    string? ExerciseShortDescription,
    StatusStudentExercise Status,
    int? Mark,
    DateTime? RequestedCheckAt,
    DateTime? CheckedAt
);
