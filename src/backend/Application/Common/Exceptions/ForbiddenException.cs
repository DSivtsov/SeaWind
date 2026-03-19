namespace Application.Common.Exceptions;

// Пользователь аутентифицирован, но не имеет прав
public class ForbiddenException : Exception
{
    public ForbiddenException(string? errorMessage) : base(errorMessage)
    {
    }
}
