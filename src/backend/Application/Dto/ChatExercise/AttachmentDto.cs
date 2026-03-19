namespace Application.Dto.ChatExercise;

public sealed class AttachmentDto
{
    public string? Id { get; init; }
    public string ThreadId { get; init; } = default!;
    public string? MessageId { get; init; }
    public string FileNameOriginal { get; init; } = default!;
    public string StoragePath { get; init; } = default!;
    public DateTime CreatedAt { get; init; }

    public AttachmentDto() { }
}

/*
 export type MessageDto = {
    id: string
    threadId: string
    messageId?: string
    fileName: string
    fileUrl: string
    createdAt: string
}

 */
