using Application.Dto.ChatExercise;

namespace Application.Abstractions.Repositories.ChatExercise;

public interface ILockRepository
{
    Task<LockDto?> TryAcquireLockAsync(string threadId, string ownerId, TimeSpan leaseTime, CancellationToken ct);

    Task ReleaseLockAsync(string threadId, string ownerId, CancellationToken ct);
}
