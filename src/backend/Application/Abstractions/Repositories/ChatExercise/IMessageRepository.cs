using Application.Dto.ChatExercise;

namespace Application.Abstractions.Repositories.ChatExercise;

public interface IMessageRepository
{
    Task<IReadOnlyList<ThreadMessageResponse>> GetThreadMessagesAsync(string threadId, CancellationToken ct);

    Task<(string Id, long Seq, DateTime CreatedAt)> AddNewMessageAsync(MessageDto message, CancellationToken ct);

    Task<IReadOnlyList<ThreadMessageResponse>> GetThreadMessagesAsync(string threadId, long beginSeq,
        long tillSeq, CancellationToken ct);
}
