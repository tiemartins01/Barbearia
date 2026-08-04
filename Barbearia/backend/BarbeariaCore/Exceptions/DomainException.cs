namespace Barbearia.Core.Exceptions;

public sealed class DomainException: System.Exception
{
    public string Code { get; }
    // Apenas criar as mensagens em caso de erro ou sucesso!
    public DomainException(string message) : this("DOMAIN_ERROR", message)
    {
    }
    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}
