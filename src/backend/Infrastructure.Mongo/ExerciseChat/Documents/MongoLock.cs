using Application.Dto.ChatExercise;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Mongo.ExerciseChat.Documents;

[BsonIgnoreExtraElements]
public sealed class MongoLock
{
    [BsonId]
    public ObjectId ResourceId { get; set; }

    [BsonElement("ownerId")]
    public string OwnerId { get; set; } = default!;

    [BsonElement("expiresAtUtc")]
    public DateTime ExpiresAtUtc { get; set; }

    public MongoLock() { }

    public MongoLock(LockDto msg)
    {
        ResourceId = ObjectId.Parse(msg.ResourceId);
        OwnerId = msg.OwnerId;
        ExpiresAtUtc = msg.ExpiresAtUtc;
    }

    public LockDto ToModel() => new()
    {
        ResourceId = this.ResourceId.ToString(),
        OwnerId = this.OwnerId,
        ExpiresAtUtc = this.ExpiresAtUtc,
    };
}
