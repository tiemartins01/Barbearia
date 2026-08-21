namespace BarbeariaCore.Application.Exceptions;
public sealed class PersistenceConflictException : Exception
{
    public string Code { get; }

    public PersistenceConflictException(
        string code,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }
}