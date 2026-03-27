using Api.Dtos;
using Application.Abstractions.Repositories.ChatExercise;
using Application.Common.Enums;
using Application.Dto.ChatExercise;
using Application.Dto.Exercise;

namespace Infrastructure.Mongo.ExerciseChat;

// Тесты в данный момент не используют БД Mongo
internal sealed class FakeExerciseChatRepository : IMessageRepository, IThreadRepository,
    IAttachmentRepository, ILockRepository
{
    public Task<string> CreateNewThreadAsync(CancellationToken ct) => Task.FromResult(Guid.NewGuid().ToString());

    public Task<ThreadLocksDto> GetThreadLocksDataAsync(string threadId, CancellationToken ct)
        => Task.FromResult(new ThreadLocksDto(0));

    public Task<IReadOnlyList<MessageDto>> GetMessagesAsync(string threadId, int limit = 0)
        => Task.FromResult<IReadOnlyList<MessageDto>>(Array.Empty<MessageDto>());

    public Task<MessageDto> AddMessageAsync(MessageDto message) => Task.FromResult(message);

    public Task DeleteOrphanThreads(IEnumerable<string> enumerable) => Task.CompletedTask;

    public Task<string[]> GetCandidatesThreads(DateTime cutoffUtc) => Task.FromResult(new string[0]);

    public Task<string> AddAttachmentAsync(AttachmentDto attachment, CancellationToken ct)
        => Task.FromResult("");

    public Task<AttachmentDownloadDto> GetAttachmentMetadataAsync(string attachmentId, CancellationToken ct)
        => Task.FromResult(default(AttachmentDownloadDto)!);

    public Task<bool> ValidateAttachmentsForThread(string threadId, string[] attachmentIds, CancellationToken ct)
        => Task.FromResult(false);

    public Task<long> GetServerSeqForNewMessage(string threadId, CancellationToken ct) => Task.FromResult(default(long));

    public Task<(string Id, long Seq, DateTime CreatedAt)> AddNewMessageAsync(MessageDto message, CancellationToken ct)
        => Task.FromResult(default((string Id, long Seq, DateTime CreatedAt)));

    public Task<IReadOnlyList<ThreadMessageResponse>> GetThreadMessagesAsync(string threadId, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<ThreadMessageResponse>>(Array.Empty<ThreadMessageResponse>());

    public Task<IReadOnlyList<ThreadAttachmentWithMessageIdDto>> GetThreadAttachmentsWithMessageIdAsync(string threadId,
        CancellationToken ct)
        => Task.FromResult<IReadOnlyList<ThreadAttachmentWithMessageIdDto>>(Array.Empty<ThreadAttachmentWithMessageIdDto>());

    public Task<IReadOnlyList<ThreadMessageResponse>> GetThreadMessagesAsync(string threadId, long beginSeq,
        long tillSeq, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<ThreadMessageResponse>>(Array.Empty<ThreadMessageResponse>());

    public Task AttachToMessageAsync(string id, string[] attachmentIds, CancellationToken ct)
        => Task.CompletedTask;

    public Task<ChangeStatusThreadResponse> UpdateThreadLockSeq(string threadId, ExerciseChatRole chatRole,
        CancellationToken ct) => Task.FromResult(default(ChangeStatusThreadResponse)!);

    public Task<LockDto?> TryAcquireLockAsync(string threadId, string ownerId, TimeSpan leaseTime,
        CancellationToken ct) => Task.FromResult(default(LockDto));

    public Task ReleaseLockAsync(string threadId, string ownerId, CancellationToken ct)
        => Task.CompletedTask;
}
