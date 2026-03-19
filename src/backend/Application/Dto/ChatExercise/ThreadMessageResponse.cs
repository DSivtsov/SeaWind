using Application.Common.Enums;

namespace Application.Dto.ChatExercise;

public sealed record ThreadAttachmentWithMessageIdDto(
    string Id,
    string MessageId,
    string FileNameOriginal    
);

public sealed record ThreadAttachmentResponse(
    string Id,
    string FileNameOriginal
);

public sealed record ThreadMessageResponse(
    string Id,
    long Seq,
    string AuthorId,
    ExerciseChatRole AuthorRole,
    string? Text,
    DateTime CreatedAt,
    ThreadAttachmentResponse[] Attachments
);
