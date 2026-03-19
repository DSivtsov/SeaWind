using Api.Dtos;
using Application.Abstractions.Repositories.ChatExercise;
using Application.Common.Enums;
using Application.Dto.Exercise;
using Infrastructure.Mongo.ExerciseChat.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Mongo.ExerciseChat.Repositories;

public sealed class MongoThreadRepository : IThreadRepository
{
    private readonly IMongoCollection<MongoThread> _threads;

    public MongoThreadRepository(IExerciseChatDb db)
    {
        _threads = db.Threads;
    }

    public async Task<ThreadLocksDto> GetThreadLocksDataAsync(string threadId, CancellationToken ct)
    {
        var id = ObjectId.Parse(threadId);
        var docs = await _threads
            .Find(x => x.Id == id)
            .Project(x => new ThreadLocksDto(x.LockSeq))
            .ToListAsync(ct);

        var count = docs.Count;
        if (count != 1)
        {
            throw new InvalidOperationException(count == 0 ? $"Thread not found: {threadId}" :
                $"ChatThread invariant violated: expected 1 thread for id {threadId}, got {count}");
        }

        return new ThreadLocksDto(docs[0].LockSeq);
    }

    public async Task<string> CreateNewThreadAsync(CancellationToken ct)
    {
        var doc = new MongoThread();
        await _threads.InsertOneAsync(doc, null, ct);

        // После InsertOneAsync поле Id получит от сервера ObjectId
        return doc.Id.ToString();
    }

    public async Task<string[]> GetCandidatesThreads(DateTime cutoffUtc)
    {
        // Find old threads (candidate orphans)
        var filter = Builders<MongoThread>.Filter.Lt(x => x.CreatedAt, cutoffUtc);

        // Limit batch size to keep it safe/simple.
        var list = await _threads
                .Find(filter)
                .Project(x => x.Id.ToString())
                .ToListAsync();

        return list.ToArray();
    }

    public async Task DeleteOrphanThreads(IEnumerable<string> orphanThreads)
    {
        var orphanObjectId = orphanThreads.Select(id => ObjectId.Parse(id)).ToList();
        var delFilter = Builders<MongoThread>.Filter.In("_id", orphanObjectId);
        await _threads.DeleteManyAsync(delFilter);
    }

    public async Task<long> GetServerSeqForNewMessage(string threadId, CancellationToken ct)
    {
        var threadObjectId = ObjectId.Parse(threadId);
        var filter = Builders<MongoThread>.Filter.Eq(x => x.Id, threadObjectId);
        var update = Builders<MongoThread>.Update.Inc(x => x.LastMessageSeq, 1);

        var options = new FindOneAndUpdateOptions<MongoThread, MongoThread>()
        {
            ReturnDocument = ReturnDocument.After
        };

        var thread = await _threads
                        .FindOneAndUpdateAsync(filter, update, options, ct);

        if (thread is null)
            throw new InvalidOperationException("Chat thread not found.");

        return thread.LastMessageSeq;
    }

    public async Task<ChangeStatusThreadResponse> UpdateThreadLockSeq(string threadId, ExerciseChatRole chatRole,
        CancellationToken ct)
    {
        var threadObjectId = ObjectId.Parse(threadId);
        var filter = Builders<MongoThread>.Filter.Eq(x => x.Id, threadObjectId);

        var pipeline = new[]
        {
            new BsonDocument("$set",
                new BsonDocument("lockSeq", "$lastMessageSeq"))
        };

        var update = Builders<MongoThread>.Update.Pipeline(pipeline);

        var options = new FindOneAndUpdateOptions<MongoThread, MongoThread>
        {
            ReturnDocument = ReturnDocument.After
        };

        var thread = await _threads.FindOneAndUpdateAsync(
            filter,
            update,
            options,
            ct);

        if (thread is null)
            throw new InvalidOperationException("Chat thread not found.");

        return new ChangeStatusThreadResponse(thread.LastMessageSeq);
    }
}
