namespace Api.Exceptions;

// Пользователь аутентифицирован, но не имеет прав
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string? errorMessage) : base(errorMessage)
    {
    }
}
