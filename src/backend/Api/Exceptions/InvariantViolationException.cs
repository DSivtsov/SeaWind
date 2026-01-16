namespace Api.Exceptions;

// сбой инварианта системы
public class InvariantViolationException : Exception
{
    public InvariantViolationException (string? errorMessage) : base(errorMessage)
    {
    }
}
