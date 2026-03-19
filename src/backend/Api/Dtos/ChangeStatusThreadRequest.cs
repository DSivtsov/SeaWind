using Application.Common.Enums;

namespace Api.Dtos;

public sealed class ChangeStatusThreadRequest
{
    public StatusStudentExercise NewStatus { get; init; }

    public int? Mark { get; init; }
}
