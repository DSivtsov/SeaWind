namespace Api.Exceptions;

internal class ExceptionToStatusCodeMap
{
    internal static readonly Dictionary<Type, int> StatusCodeValues = new()
    {
        { typeof(BadRequestException), StatusCodes.Status400BadRequest },
        { typeof(UnauthorizedAccessException), StatusCodes.Status403Forbidden },
        { typeof(NotFoundException), StatusCodes.Status404NotFound },
        { typeof(ConflictException), StatusCodes.Status409Conflict },
    };
}