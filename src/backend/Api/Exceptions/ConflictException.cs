namespace Api.Exceptions;

// Конфликт такие данные уже есть
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
