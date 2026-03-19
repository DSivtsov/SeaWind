using Api.Dtos;
using Application.Abstractions.Repositories.ChatExercise;
using Application.Common.Exceptions;
using Application.Dto.ChatExercise;
using Infrastructure.Mongo.ExerciseChat.Documents;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;

namespace Infrastructure.Mongo.ExerciseChat.Repositories;

public sealed class MongoAttachmentRepository : IAttachmentRepository
{
    private readonly IMongoCollection<MongoAttachment> _attachments;

    public MongoAttachmentRepository(IExerciseChatDb db)
    {
        _attachments = db.Attachments;
    }

    public async Task<string> AddAttachmentAsync(AttachmentDto message, CancellationToken ct)
    {
        var doc = new MongoAttachment(message);
        await _attachments.InsertOneAsync(doc, options: null, ct);

        return doc.Id.ToString();
    }

    public async Task<AttachmentDownloadDto> GetAttachmentMetadataAsync(string attachmentId, CancellationToken ct)
    {
        ObjectId attachmentObjectId;
        try
        {
            attachmentObjectId = ObjectId.Parse(attachmentId);
        }
        catch
        {
            throw new ValidationException($"Неправильный attachment id [{attachmentId}].");
        }

        try
        {
            return await _attachments
                .Find(x => x.Id == attachmentObjectId)
                .Project(x => new AttachmentDownloadDto(x.FileNameOriginal, x.StoragePath))
                .SingleAsync(ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvariantViolationException($"Не удалось загрузить метаданные вложения [{attachmentId}].", ex);
        }
    }

    public async Task<IReadOnlyList<ThreadAttachmentWithMessageIdDto>>
        GetThreadAttachmentsWithMessageIdAsync(string threadId, CancellationToken ct)
    {
        var threadObjectId = ObjectId.Parse(threadId);
        var docs = await _attachments
                    .Find(x => x.ThreadId == threadObjectId && x.MessageId != null)
                    .ToListAsync(ct);

        return docs
                .Select(doc => new ThreadAttachmentWithMessageIdDto(
                    doc.Id.ToString(),
                    doc.MessageId.ToString()!,
                    doc.FileNameOriginal
                ))
                .ToList();
    }

    public Task AttachToMessageAsync(string messageId, string[] attachmentIds, CancellationToken ct)
    {
        var messageObjectId = ObjectId.Parse(messageId);
        var attachmentObjectIds = attachmentIds.Select(id => ObjectId.Parse(id));

        var filter = Builders<MongoAttachment>.Filter.In(rec => rec.Id, attachmentObjectIds);
        var update = Builders<MongoAttachment>.Update.Set(rec => rec.MessageId, messageObjectId);
        
        return _attachments.UpdateManyAsync(filter, update, null, ct);
    }

    public async Task<bool> ValidateAttachmentsForThread(string threadId, string[] attachmentIds,
        CancellationToken ct)
    {
        var attachmentObjectIds = new HashSet<ObjectId>(attachmentIds.Select(x => ObjectId.Parse(x)));
        var threadObjectId = ObjectId.Parse(threadId);
        var countCorrect = await _attachments
            .Find(x => attachmentObjectIds.Contains(x.Id) && x.ThreadId == threadObjectId && x.MessageId == null)
            .CountDocumentsAsync(ct);

        return countCorrect == attachmentObjectIds.Count;
    }
}
