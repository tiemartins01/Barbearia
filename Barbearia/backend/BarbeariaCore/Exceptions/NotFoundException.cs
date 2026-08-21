namespace BarbeariaCore.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string code, string message) : base(code, message)
        {

        }
    }
}