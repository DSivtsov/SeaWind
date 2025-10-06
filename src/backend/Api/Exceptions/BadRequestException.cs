namespace Api.Exceptions;

// Неверный запрос
public class BadRequestException : Exception
{
    public BadRequestException(string? errorMessage) : base(errorMessage)
    {
    }
}
