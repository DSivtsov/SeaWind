using Api.Dtos;

namespace Application.Abstractions.Services.ChatExercise;

public interface IAttachmentService
{
    Task<List<AttachmentUploadResponse>> SaveUploadedAttachmentsAsync(List<ThreadAttachmentsDto> dtos,
        CancellationToken ct);
}
