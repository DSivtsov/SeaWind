namespace Application.Common.Exceptions;

// Неверный запрос
public class ValidationException : Exception
{
    public ValidationException(string? errorMessage) : base(errorMessage)
    {
    }
}
