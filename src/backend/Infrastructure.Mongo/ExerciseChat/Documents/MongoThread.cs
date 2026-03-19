using Application.Dto.ChatExercise;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Mongo.ExerciseChat.Documents;

[BsonIgnoreExtraElements]
public sealed class MongoThread
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("lastMessageSeq")]
    public long LastMessageSeq { get; set; } = 0;

    [BsonElement("lockSeq")]
    public long LockSeq { get; set; } = 0;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MongoThread() { }

    public ThreadDto ToModel() => new()
    {
        Id = this.Id.ToString(),
        LastMessageSeq = this.LastMessageSeq,
        LockSeq = this.LockSeq,
        CreatedAt = this.CreatedAt,
    };
}
