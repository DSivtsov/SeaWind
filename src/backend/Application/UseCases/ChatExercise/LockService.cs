using Application.Abstractions.Repositories.ChatExercise;
using Application.Abstractions.Services.ChatExercise;

namespace Application.UseCases.ChatExercise
{
    internal class LockService : ILockService
    {
        private static readonly TimeSpan leaseTime = TimeSpan.FromMilliseconds(2000);
        private readonly ILockRepository _lockRepository;

        public LockService(ILockRepository lockRepository)
        {
            _lockRepository = lockRepository;
        }

        public async Task<bool> AcquireLockAsync(string threadId, string lockOwnerId, CancellationToken ct)
        {
            var lockThread = await _lockRepository.TryAcquireLockAsync(threadId, lockOwnerId, leaseTime, ct);

            return lockThread != null && lockThread.OwnerId == lockOwnerId;
        }

        public Task ReleaseLockAsync(string threadId, string lockOwnerId, CancellationToken ct)
        {
            return _lockRepository.ReleaseLockAsync(threadId, lockOwnerId, ct);
        }
    }
}
