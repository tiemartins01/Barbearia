namespace BarbeariaCore.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(string code, string message)
        : base(code, message)
    {
    }
}
