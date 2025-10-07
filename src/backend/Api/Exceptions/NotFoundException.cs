namespace Api.Exceptions;

// Ресурс не найден
public class NotFoundException : Exception
{
    public NotFoundException(string? errorMessage) : base(errorMessage)
    {
    }
}
