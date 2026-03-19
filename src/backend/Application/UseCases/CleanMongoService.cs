using Application.Abstractions.Repositories;
using Application.Abstractions.Repositories.ChatExercise;

namespace Application.UseCases;

public sealed class CleanMongoService
{
    private readonly IThreadRepository _threadRepository;
    private readonly IStudentExerciseRepository _studentExerciseRepository;

    public CleanMongoService(IThreadRepository threadRepository,
        IStudentExerciseRepository studentExerciseRepository)
    {
        _threadRepository = threadRepository;
        _studentExerciseRepository = studentExerciseRepository;
    }

    public async Task<bool> OrphanThreadsAsync(DateTime utcNow, TimeSpan ttl, int limitBatch, int batchSecDelay,
        CancellationToken stoppingToken)
    {
        var cutoffUtc = utcNow - ttl;

        var candidatesThreadIds = await _threadRepository.GetCandidatesThreads(cutoffUtc);

        if (candidatesThreadIds.Length == 0) // удалять нечего
            return true;

        for (int i = 0; i < candidatesThreadIds.Length; i += limitBatch)
        {
            var size = Math.Min(limitBatch, candidatesThreadIds.Length - i);

            var batch = new string[size];
            Array.Copy(candidatesThreadIds, i, batch, 0, size);

            var used = await _studentExerciseRepository.GetUsedThreads(batch);

            var orphan = batch.Where(id => !used.Contains(id)).ToArray();

            if (orphan.Length > 0)
            {
                await _threadRepository.DeleteOrphanThreads(orphan);

                await Task.Delay(TimeSpan.FromSeconds(batchSecDelay), stoppingToken);
            }
        }

        return true; // было что удалять
    }
}
