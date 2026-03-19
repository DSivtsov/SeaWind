using Application.Abstractions.Repositories.ChatExercise;
using Application.Dto.ChatExercise;
using Infrastructure.Mongo.ExerciseChat.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Mongo.ExerciseChat.Repositories;

public sealed class MongoLockRepository : ILockRepository
{
    private readonly IMongoCollection<MongoLock> _locks;

    public MongoLockRepository(IExerciseChatDb db)
    {
        _locks = db.Locks;
    }

    public async Task<LockDto?> TryAcquireLockAsync(string threadId, string ownerId, DateTime utcNow,
        TimeSpan leaseTime, CancellationToken ct)
    {
        var threadObjectId = ObjectId.Parse(threadId);
        var resourceId = ObjectId.Parse(threadId);
        var now = DateTime.UtcNow;
        var expiresAtUtc = now.Add(leaseTime);

        FilterDefinitionBuilder<MongoLock> f = Builders<MongoLock>.Filter;
        FilterDefinition<MongoLock> filter =
            f.Eq(x => x.ResourceId, resourceId) &
            f.Or(f.Eq(x => x.OwnerId, ownerId), f.Lte(x => x.ExpiresAtUtc, now));

        UpdateDefinition<MongoLock> update =
            Builders<MongoLock>.Update
                .Set(x => x.OwnerId, ownerId)
                .Set(x => x.ExpiresAtUtc, expiresAtUtc)
                .SetOnInsert(x => x.ResourceId, resourceId);

        FindOneAndUpdateOptions<MongoLock> options = new()
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        try
        {
            var mongoLock = await _locks.FindOneAndUpdateAsync(filter, update, options, ct);
            return mongoLock.ToModel();
        }
        catch (MongoDuplicateKeyException)
        {
            return null;
        }
    }

    public async Task ReleaseLockAsync(string threadId, string ownerId, CancellationToken ct)
    {
        var resourceId = ObjectId.Parse(threadId);

        FilterDefinitionBuilder<MongoLock> f = Builders<MongoLock>.Filter;
        FilterDefinition<MongoLock> filter =
            f.Eq(x => x.ResourceId, resourceId) &
            f.Eq(x => x.OwnerId, ownerId);

        await _locks.DeleteOneAsync(filter, ct);
    }
}
