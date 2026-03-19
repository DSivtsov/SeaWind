using Application.Common.Enums;
using Application.Dto.ChatExercise;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Mongo.ExerciseChat.Documents;

[BsonIgnoreExtraElements]
public sealed class MongoMessage
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("threadId")]
    public ObjectId ThreadId { get; init; } = default!;

    [BsonElement("seq")]
    public long Seq { get; set; }

    [BsonElement("authorId")]
    public string AuthorId { get; init; } = default!;

    [BsonElement("authorRole")]
    public string AuthorRole { get; init; } = default!;

    [BsonElement("text")]
    public string? Text { get; init; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("attachmentIds")]
    public string[] AttachmentIds { get; init; } = [];

    public MongoMessage() { }

    public MongoMessage(MessageDto msg)
    {
        ThreadId = ObjectId.Parse(msg.ThreadId);
        Seq = msg.Seq;
        AuthorId = msg.AuthorId;
        AuthorRole = msg.AuthorRole.ToString();
        Text = msg.Text;
        CreatedAt = DateTime.UtcNow;
        AttachmentIds = msg.AttachmentIds;
    }

    public MessageDto ToModel() => new()
    {
        Id = this.Id.ToString(),
        ThreadId = this.ThreadId.ToString(),
        Seq = this.Seq,
        AuthorId = this.AuthorId,
        AuthorRole = Enum.Parse<ExerciseChatRole>(this.AuthorRole),
        Text = this.Text,
        CreatedAt = this.CreatedAt,
        AttachmentIds = this.AttachmentIds
    };


}

