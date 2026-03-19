using Application.Dto.ChatExercise;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Mongo.ExerciseChat.Documents;

[BsonIgnoreExtraElements]
public sealed class MongoAttachment
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("threadId")]
    public ObjectId ThreadId { get; init; } = default!;

    [BsonElement("messageId")]
    public ObjectId? MessageId { get; set; }

    [BsonElement("fileNameOriginal")]
    public string FileNameOriginal { get; set; } = default!;

    [BsonElement("storagePath")]
    public string StoragePath { get; set; } = default!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    public MongoAttachment() { }

    public MongoAttachment(AttachmentDto msg)
    {
        ThreadId = ObjectId.Parse(msg.ThreadId);
        FileNameOriginal = msg.FileNameOriginal;
        StoragePath = msg.StoragePath;
        CreatedAt = DateTime.UtcNow;
    }

    public AttachmentDto ToModel() => new()
    {
        Id = this.Id.ToString(),
        ThreadId = this.ThreadId.ToString(),
        MessageId = this.MessageId.ToString(),
        FileNameOriginal = this.FileNameOriginal,
        StoragePath = this.StoragePath,
        CreatedAt = this.CreatedAt,
    };


}

