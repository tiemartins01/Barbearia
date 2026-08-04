namespace Barbearia.Core.Exceptions;

public abstract class AppException : System.Exception
{
    public string Code { get; }

    protected AppException(string code, string message)
        : base(message)
    {
        Code = code;
    }
}
