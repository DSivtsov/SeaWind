using Application.Common.Exceptions;

namespace Api.Filters;

internal class ExceptionToStatusCodeMap
{
    internal static readonly Dictionary<Type, int> StatusCodeValues = new()
    {
        { typeof(ValidationException), StatusCodes.Status400BadRequest },
        { typeof(UnauthorizedException), StatusCodes.Status401Unauthorized },
        { typeof(ForbiddenException), StatusCodes.Status403Forbidden },
        { typeof(NotFoundException), StatusCodes.Status404NotFound },
        { typeof(ConflictException), StatusCodes.Status409Conflict },
        { typeof(InvariantViolationException), StatusCodes.Status500InternalServerError },
    };
}
