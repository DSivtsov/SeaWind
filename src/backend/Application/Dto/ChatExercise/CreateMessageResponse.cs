namespace Application.Dto.ChatExercise;

public sealed record CreateMessageResponse(long ClientSeq, string MessageId, long ServerSeq, DateTime CreatedAt);
