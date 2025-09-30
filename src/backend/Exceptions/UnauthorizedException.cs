namespace Backend.Exceptions;

// Пользователь не авторизован
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string? errorMessage) : base(errorMessage)
    {
    }
}
