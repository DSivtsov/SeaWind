namespace Application.Dto.ChatExercise;

public sealed class ThreadDto
{
    public string? Id { get; init; }
    public long LastMessageSeq { get; init; } =0;
    public long LockSeq { get; init; } = 0;
    public DateTime CreatedAt { get; init; }

    public ThreadDto() { }
}

