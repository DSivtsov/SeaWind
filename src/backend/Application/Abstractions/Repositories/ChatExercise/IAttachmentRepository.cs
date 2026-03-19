using Api.Dtos;
using Application.Dto.ChatExercise;

namespace Application.Abstractions.Repositories.ChatExercise;

public interface IAttachmentRepository
{
    Task<string> AddAttachmentAsync(AttachmentDto attachment, CancellationToken ct);

    Task<AttachmentDownloadDto> GetAttachmentMetadataAsync(string attachmentId, CancellationToken ct);

    Task<IReadOnlyList<ThreadAttachmentWithMessageIdDto>> GetThreadAttachmentsWithMessageIdAsync(string threadId,
        CancellationToken ct);
    Task AttachToMessageAsync(string id, string[] attachmentIds, CancellationToken ct);

    Task<bool> ValidateAttachmentsForThread(string threadId, string[] attachmentIds, CancellationToken ct);
}
