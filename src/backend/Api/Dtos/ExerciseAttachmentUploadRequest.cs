using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public sealed class ExerciseAttachmentUploadRequest
{
    [Required]
    public List<IFormFile> Files { get; init; } = default!;
}
