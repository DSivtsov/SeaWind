namespace Api.Exceptions;

// Пользователь не аутентифицирован или ввёл неверные учётные данные
// (нет токена, токен просрочен, подпись неверна)
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string? errorMessage) : base(errorMessage)
    {
    }
}
