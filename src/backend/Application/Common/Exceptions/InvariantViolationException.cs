namespace Application.Common.Exceptions;

// сбой инварианта системы
public class InvariantViolationException : Exception
{
    public InvariantViolationException(string? errorMessage) : base(errorMessage)
    {
    }

    public InvariantViolationException(string? message, Exception innerException) : base(message, innerException)
    {
    }
}
