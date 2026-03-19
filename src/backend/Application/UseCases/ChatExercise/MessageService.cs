using Application.Abstractions.Repositories;
using Application.Abstractions.Repositories.ChatExercise;
using Application.Abstractions.Services.ChatExercise;
using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Dto.ChatExercise;
using System.Threading;

namespace Application.UseCases.ChatExercise;

public sealed class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IThreadRepository _threadRepository;
    private readonly ILockService _lockService;
    private readonly IStudentExerciseRepository _studentExerciseRepository;

    public MessageService(IMessageRepository messageRepository, IAttachmentRepository attachmentRepository,
        IThreadRepository threadRepository, ILockService lockService, IStudentExerciseRepository studentExerciseRepository)
    {
        _messageRepository = messageRepository;
        _attachmentRepository = attachmentRepository;
        _threadRepository = threadRepository;
        _lockService = lockService;
        _studentExerciseRepository = studentExerciseRepository;
    }

    public async Task<CreateMessageResponse> ValidateAndSaveMessageAsync(CreateMessageCommand cmd,
        CancellationToken ct)
    {
        string[] attachmentIds = cmd.AttachmentIds;

        var threadId = cmd.ThreadId;
        var isAttachmentValid = await _attachmentRepository.ValidateAttachmentsForThread(threadId,
            attachmentIds, ct);

        if (!isAttachmentValid)
            throw new ValidationException("Часть вложений в отправленном сообщении недействительна.");

        if (!Enum.TryParse(cmd.AuthorRole, true, out ExerciseChatRole chatRole))
            throw new ValidationException($"Недопустимая роль [{cmd.AuthorRole}] в Exercise чате");

        var lockOwnerId = Guid.NewGuid().ToString();
        var locked = await _lockService.AcquireLockAsync(threadId, lockOwnerId, ct);
        if (!locked)
            throw new ConflictException($"Чат {threadId} пока занят другими операциями записи.");

        var authorId = cmd.AuthorId;
        try
        {
            var hasWriteAccess = await _studentExerciseRepository.HasAuthorWriteAccessAsync(threadId, chatRole, ct);

            if (!hasWriteAccess)
                throw new ForbiddenException("В данный момент запись в чат не возможна.");

            var serverSeq = await _threadRepository.GetServerSeqForNewMessage(threadId, ct);

            var newMessage = new MessageDto()
            {
                ThreadId = threadId,
                Seq = serverSeq,
                AuthorId = authorId,
                AuthorRole = chatRole,
                Text = cmd.Text,
                AttachmentIds = attachmentIds,
            };

            var resSrv = await _messageRepository.AddNewMessageAsync(newMessage, ct);

            if (attachmentIds.Length != 0)
                await _attachmentRepository.AttachToMessageAsync(resSrv.Id, attachmentIds, ct);

            return new CreateMessageResponse(cmd.ClientSeq, resSrv.Id, resSrv.Seq, resSrv.CreatedAt);
        }
        finally
        {
            await _lockService.ReleaseLockAsync(threadId, lockOwnerId, CancellationToken.None);
        }
    }
}
