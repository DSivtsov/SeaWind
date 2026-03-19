namespace Api.Dtos;

public sealed class ExerciseMessageUploadRequest
{
    public long ClientSeq { get; init; }

    public string? Text { get; init; }

    public string[] AttachmentIds { get; init; } = [];
}
