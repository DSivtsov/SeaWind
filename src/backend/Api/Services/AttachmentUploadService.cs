using Api.Configuration;
using Api.Dtos;
using Application.Common.Exceptions;
using Microsoft.Extensions.Options;

namespace Api.Services;

public sealed class AttachmentUploadService
{
    private static readonly string[] AllowedExtensions =
    [
        ".png", ".jpg", ".jpeg", ".pdf", ".zip", ".txt", ".md", ".cs"
    ];

    public const long MaxRequestSizeBytes = 10 * 1024 * 1024;
    public const long MaxNumFilesPerRequest = 5;

    private readonly IWebHostEnvironment _env;
    private readonly StorageOptions _storage;

    public AttachmentUploadService(IWebHostEnvironment env, IOptions<StorageOptions> options)
    {
        _env = env;
        _storage = options.Value;
    }

    public async Task<List<ThreadAttachmentsDto>> UploadAsync(string threadId, List<IFormFile> files, CancellationToken ct)
    {
        if (files.Count == 0)
            throw new ValidationException("File list is empty.");

        if (files.Count > MaxNumFilesPerRequest)
            throw new ValidationException($"Too many files." +
                $" Maximum number {MaxNumFilesPerRequest} files per one request");

        var getFileTaks = files.Select(file => UploadFileAsync(threadId, file, ct));

        var uploadFileResultDtos = await Task.WhenAll(getFileTaks);

        return uploadFileResultDtos.ToList();
    }

    private async Task<ThreadAttachmentsDto> UploadFileAsync(string threadId, IFormFile file,
        CancellationToken ct)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("File is empty.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Unsupported file type.");

        var webRootPath = _env.WebRootPath;
        var safeFileName = $"{Guid.NewGuid():N}{ext}";

        var relativeFolder = Path.Combine(_storage.ExerciseAttachmentsFolder, threadId);
        var absoluteFolder = Path.Combine(webRootPath, relativeFolder);

        Directory.CreateDirectory(absoluteFolder);

        var absolutePath = Path.Combine(absoluteFolder, safeFileName);

        await using var stream = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(stream, ct);

        var storagePath = $"{threadId}/{safeFileName}";

        return new ThreadAttachmentsDto(threadId, file.FileName, storagePath);
    }
}
