using Api.Dtos;
using Application.Common.Enums;
using Application.Dto.ChatExercise;
using Application.Dto.Exercise;

namespace Application.Abstractions.Repositories.ChatExercise;

public interface IThreadRepository
{
    Task<ThreadLocksDto> GetThreadLocksDataAsync(string threadId, CancellationToken ct);

    Task<string> CreateNewThreadAsync(CancellationToken ct);

    Task<string[]> GetCandidatesThreads(DateTime cutoffUtc);

    Task DeleteOrphanThreads(IEnumerable<string> enumerable);

    Task<long> GetServerSeqForNewMessage(string threadId, CancellationToken ct);

    Task<ChangeStatusThreadResponse> UpdateThreadLockSeq(string threadId, ExerciseChatRole chatRole,
        CancellationToken ct);
}
