using Application.Common.Enums;

namespace Application.Dto.ChatExercise;

public sealed class MessageDto
{
    public string? Id { get; init; }
    public string ThreadId { get; init; } = default!;
    public long Seq { get; init; }
    public string AuthorId { get; init; } = default!;
    public ExerciseChatRole AuthorRole { get; init; } = default!;
    public string? Text { get; init; }
    public DateTime CreatedAt { get; init; }
    public string[] AttachmentIds { get; init; } = [];

    public MessageDto() { }
}
