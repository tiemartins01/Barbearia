namespace BarbeariaCore.Exceptions;
public sealed class AuthenticationException : AppException
{
    public AuthenticationException(string code, string message)
        : base(code, message)
    {
    }
}