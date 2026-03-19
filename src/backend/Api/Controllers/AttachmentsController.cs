using Application.Abstractions.Repositories.ChatExercise;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private const string FOLDER_ATTACHMENT_STORAGE = "chatsExercise-attachments";
    private readonly ILogger<AttachmentsController> _logger;

    public AttachmentsController(ILogger<AttachmentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Скачивание вложения чата по его идентификатору.
    /// </summary>
    /// <remarks>
    /// Метод получает метаданные вложения из репозитория, проверяет наличие файла
    /// в файловом хранилище и возвращает его как скачиваемый файл.
    /// Если метаданные вложения или сам файл не найдены — возвращается 404.
    /// </remarks>
    /// <param name="attachmentId">Идентификатор вложения.</param>
    /// <param name="_env">Среда выполнения приложения (используется для определения пути к wwwroot).</param>
    /// <param name="_attachmentRepository">Репозиторий вложений для получения метаданных файла.</param>
    /// <param name="ct">Токен отмены запроса.</param>
    /// <returns>
    /// Файл вложения с оригинальным именем.
    /// </returns>
    [HttpGet("{attachmentId}/download")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachmentAsync(
        [FromRoute] string attachmentId,
        IWebHostEnvironment _env,
        IAttachmentRepository _attachmentRepository,
        CancellationToken ct)
    {
        var dto = await _attachmentRepository.GetAttachmentMetadataAsync(attachmentId, ct);

        string storagePath = dto.StoragePath;

        var relativeFilePath = Path.Combine(FOLDER_ATTACHMENT_STORAGE, storagePath);
        var absoluteFilePath = Path.Combine(_env.WebRootPath, relativeFilePath);

        if (!System.IO.File.Exists(absoluteFilePath))
        {
            _logger.LogError("Attachment file not found: {FilePath}", absoluteFilePath);

            throw new NotFoundException($"Файл вложения [{dto.FileNameOriginal}] не найден в хранилище.");
        }

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(storagePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return PhysicalFile(absoluteFilePath, contentType, dto.FileNameOriginal);
    }

}
