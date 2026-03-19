namespace Application.Dto.ChatExercise;

public sealed class LockDto
{
    public string ResourceId { get; init; } = default!;
    public string OwnerId { get; init; } = default!;
    public DateTime ExpiresAtUtc { get; init; }

    public LockDto() { }
}

