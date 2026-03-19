using Api.Dtos;
using Api.Services;
using Application.Abstractions.Repositories.ChatExercise;
using Application.Abstractions.Services;
using Application.Abstractions.Services.ChatExercise;
using Application.Dto.ChatExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class ThreadsController : ControllerBase
{
    private readonly IMessageService _service;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IThreadService _threadService;
    private readonly ICurrentUserService _userService;

    public ThreadsController(IMessageService service, IAttachmentRepository attachmentRepository,
        IMessageRepository messageRepository, ICurrentUserService currentUserService,
        IThreadService threadService, ICurrentUserService userService)
    {
        _service = service;
        _attachmentRepository = attachmentRepository;
        _messageRepository = messageRepository;
        _currentUserService = currentUserService;
        _threadService = threadService;
        _userService = userService;
    }

    /// <summary>
    /// Получение списка сообщений треда вместе с вложениями.
    /// </summary>
    /// <remarks>
    /// Возвращает сообщения указанного треда, отсортированные по порядковому номеру (Seq).
    /// Для каждого сообщения в ответе присутствует список вложений с URL для скачивания.
    /// Если сообщений в треде нет — возвращается пустой список.
    /// </remarks>
    /// <param name="threadId">Идентификатор треда.</param>
    /// <param name="query">
    /// Диапазон загрузки сообщений:
    /// Указать с какого и до какого номера сообщения.
    /// </param>
    /// <param name="ct">Токен отмены HTTP-запроса.</param>
    /// <returns>Список сообщений треда.</returns>
    [HttpGet("{threadId}/messages")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<ThreadMessageResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ThreadMessageResponse>>> GetThreadMessagesAsync(string threadId,
        [FromQuery] MessagesQueryDto query,
        CancellationToken ct)
    {
        var currentUser = _currentUserService.GetUserInfo();

        await _threadService.CheckThreadAccessAsync(threadId, currentUser, ct);

        IReadOnlyList<ThreadMessageResponse> messages;
        if (query.HasNoRangeSet())
        {
            messages = await _messageRepository.GetThreadMessagesAsync(threadId, ct);

            if (messages.Count == 0)
                return Ok(Array.Empty<ThreadMessageResponse>());
        }
        else
        {
            messages = await _messageRepository.GetThreadMessagesAsync(threadId, (long)query.BeginSeq!,
                (long)query.TillSeq!, ct);
        }

        var attachmentsWithMessageId = await _attachmentRepository.GetThreadAttachmentsWithMessageIdAsync(threadId, ct);

        Dictionary<string, List<ThreadAttachmentWithMessageIdDto>> attachmentsByMessageId = attachmentsWithMessageId
            .GroupBy(a => a.MessageId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = messages.Select(m => new ThreadMessageResponse(
            m.Id,
            m.Seq,
            m.AuthorId,
            m.AuthorRole,
            m.Text,
            m.CreatedAt,
            attachmentsByMessageId.TryGetValue(m.Id, out var attachments)
                ? attachments.Select(a => new ThreadAttachmentResponse(
                    a.Id,
                    a.FileNameOriginal
                )).ToArray()
                : []
        )).ToList();

        return Ok(result);
    }

    /// <summary>
    /// Создание нового сообщения в Exercise чате.
    /// </summary>
    /// <remarks>
    /// Метод сохраняет сообщение в указанном thread.
    /// Сообщение может содержать текст и список идентификаторов ранее загруженных вложений.
    /// В случае ошибки валидации (например, некорректные данные сообщения)
    /// возвращается ошибка 400.
    /// </remarks>
    /// <param name="threadId">Идентификатор чата (thread).</param>
    /// <param name="req">Данные сообщения: текст и идентификаторы вложений.</param>
    /// <param name="ct">Токен отмены запроса.</param>
    /// <returns>
    /// MessageId созданного сообщения.
    /// </returns>
    [HttpPost("{threadId}/messages")]
    [Authorize]
    [ProducesResponseType(typeof(CreateMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateMessageResponse>> CreateNewMessageExerciseChatAsync(
        [FromRoute] string threadId,
        [FromBody] ExerciseMessageUploadRequest req,
        CancellationToken ct)
    {
        var author = _userService.GetUserInfo();
        
        var cmd = new CreateMessageCommand(req.ClientSeq, threadId, author.UserId, author.Role,
            req.Text, req.AttachmentIds);
        
        var response = await _service.ValidateAndSaveMessageAsync(cmd, ct);

        return Ok(response);
    }

    /// <summary>
    /// Загружает файлы вложений для чата потока.
    /// Сначала файлы сохраняются в файловое хранилище,
    /// затем создаются записи вложений и связываются с указанным thread.
    /// </summary>
    /// <param name="threadId">
    /// Идентификатор потока чата, к которому относятся загружаемые файлы.
    /// </param>
    /// <param name="req">
    /// multipart/form-data запрос, содержащий один или несколько файлов.
    /// </param>
    /// <param name="attachmentUploadService">
    /// Сервис загрузки файлов в файловое хранилище.
    /// </param>
    /// <param name="attachmentSaveService">
    /// Сервис создания записей вложений в базе данных.
    /// </param>
    /// <param name="ct">
    /// Токен отмены операции.
    /// </param>
    /// <returns>
    /// Список загруженных вложений с метаданными для клиента.
    /// </returns>
    [HttpPost("{threadId}/attachments")]
    [RequestSizeLimit(AttachmentUploadService.MaxRequestSizeBytes)]
    [Authorize]
    [ProducesResponseType(typeof(List<AttachmentUploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<AttachmentUploadResponse>>> UploadAndSaveExerciseAttachmentAsync(
        [FromRoute] string threadId,
        [FromForm] ExerciseAttachmentUploadRequest req,
        AttachmentUploadService attachmentUploadService,
        IAttachmentService attachmentSaveService,
        CancellationToken ct)
    {
        var savedFiles = await attachmentUploadService.UploadAsync(threadId, req.Files, ct);

        var chatMessageAttachmentDtos = await attachmentSaveService.SaveUploadedAttachmentsAsync(savedFiles, ct);

        return Ok(chatMessageAttachmentDtos);
    }

    /// <summary>
    /// Изменяет статус задания в exercise-чате.
    /// </summary>
    /// <remarks>
    /// Используется студентом и ментором для перевода задания между состояниями workflow.
    ///
    /// Типичные переходы:
    /// - Student → отправляет решение на проверку (OnMentor)
    /// - Mentor → возвращает задание студенту или принимает его
    ///
    /// Операция выполняется под блокировкой thread, чтобы исключить
    /// параллельные изменения состояния чата во время выполнения операции.
    /// </remarks>
    /// <param name="threadId">
    /// Идентификатор exercise-чата.
    /// </param>
    /// <param name="req">
    /// Запрос с новым статусом и, при необходимости, оценкой ментора.
    /// </param>
    /// <param name="ct">
    /// Токен отмены операции.
    /// </param>
    /// <returns>
    /// Результат изменения статуса задания.
    /// </returns>
    [HttpPut("{threadId}/status")]
    [Authorize]
    [ProducesResponseType(typeof(ChangeStatusThreadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ChangeStatusThreadResponse>> ChangeStatusThreadAsync(
    [FromRoute] string threadId,
    [FromBody] ChangeStatusThreadRequest req,
    CancellationToken ct)
    {
        var currentUser = _userService.GetUserInfo();
        var response = await _threadService.ChangeStatusThreadAsync(threadId, currentUser, req.NewStatus,
            req.Mark, ct);

        return Ok(response);
    }
}
