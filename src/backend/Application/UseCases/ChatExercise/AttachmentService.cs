using Api.Dtos;
using Application.Abstractions.Repositories.ChatExercise;
using Application.Abstractions.Services.ChatExercise;
using Application.Common.Exceptions;
using Application.Dto.ChatExercise;

namespace Application.UseCases.ChatExercise;

internal class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _attachmentRepository;

    public AttachmentService(IAttachmentRepository attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
    }

    public async Task<List<AttachmentUploadResponse>> SaveUploadedAttachmentsAsync(List<ThreadAttachmentsDto> dtos,
        CancellationToken ct)
    {
        try
        {
            var getInsertTask = dtos.Select(dto => InsertRec(dto, ct));

            var chatMessageAttachmentDtos = await Task.WhenAll(getInsertTask);

            return chatMessageAttachmentDtos.ToList();
        }
        catch (Exception ex)
        {
            throw new InvariantViolationException("Failed to save attachments.", ex);
        }
    }

    private async Task<AttachmentUploadResponse> InsertRec(ThreadAttachmentsDto dto, CancellationToken ct)
    {
        var rec = new AttachmentDto()
        {
            ThreadId = dto.ThreadId,
            FileNameOriginal = dto.FileNameOriginal,
            StoragePath = dto.StoragePath,
        };

        var attachmentId =  await _attachmentRepository.AddAttachmentAsync(rec, ct);

        return new AttachmentUploadResponse(attachmentId, rec.FileNameOriginal);
    }
}
