namespace Application.Abstractions.Services.ChatExercise
{
    public interface ILockService
    {
        Task<bool> AcquireLockAsync(string threadId, string lockOwnerId, CancellationToken ct);

        Task ReleaseLockAsync(string threadId, string lockOwnerId, CancellationToken ct);
    }
}
