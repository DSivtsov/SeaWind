namespace Application.Dto.ChatExercise;

public sealed record CreateMessageCommand
(
    long ClientSeq,
    string ThreadId,
    string AuthorId,
    string AuthorRole,
    string? Text,
    string[] AttachmentIds
);
