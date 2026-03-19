using Application.Abstractions.Repositories.ChatExercise;
using Application.Common.Enums;
using Application.Dto.ChatExercise;
using Infrastructure.Mongo.ExerciseChat.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Mongo.ExerciseChat.Repositories;

public sealed class MongoMessageRepository : IMessageRepository
{
    private readonly IMongoCollection<MongoMessage> _messages;

    public MongoMessageRepository(IExerciseChatDb db)
    {
        _messages = db.Messages;
    }

    private static IReadOnlyList<ThreadMessageResponse> ToThreadMessageResponse(List<MongoMessage> docs)
    {
        return docs
                .Select(doc => new ThreadMessageResponse(
                    doc.Id.ToString(),
                    doc.Seq,
                    doc.AuthorId,
                    Enum.Parse<ExerciseChatRole>(doc.AuthorRole),
                    doc.Text,
                    doc.CreatedAt,
                    []
                ))
                .ToList();
    }

    public async Task<IReadOnlyList<ThreadMessageResponse>> GetThreadMessagesAsync(string threadId, CancellationToken ct)
    {
        var threadObjectId = ObjectId.Parse(threadId);
        var docs = await _messages
            .Find(x => x.ThreadId == threadObjectId)
            .SortBy(x => x.Seq)
            .ToListAsync(ct);
        return ToThreadMessageResponse(docs);
    }

    public async Task<IReadOnlyList<ThreadMessageResponse>> GetThreadMessagesAsync(string threadId, long beginSeq,
        long tillSeq, CancellationToken ct)
    {
        var threadObjectId = ObjectId.Parse(threadId);

        var builder = Builders<MongoMessage>.Filter;
        var filterBegin = builder.Gte(x => x.Seq, beginSeq);
        var filterTill = builder.Lt(x => x.Seq, tillSeq);
        var filterThread = builder.Eq(x => x.ThreadId, threadObjectId);

        var filter = builder.And(filterBegin, filterTill,filterThread);


        var docs = await _messages
            .Find(filter)
            .SortBy(x => x.Seq)
            .ToListAsync(ct);

        return ToThreadMessageResponse(docs);
    }

    public async Task<(string Id, long Seq, DateTime CreatedAt)> AddNewMessageAsync(MessageDto message,
        CancellationToken ct)
    {
        var doc = new MongoMessage(message);

        await _messages.InsertOneAsync(doc, null, ct);

        return (doc.Id.ToString(), doc.Seq, doc.CreatedAt);
    }
}
