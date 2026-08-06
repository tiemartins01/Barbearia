using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.Domain.ValueObjects;
using Microsoft.Extensions.Hosting;

namespace BarbeariaApi.Security;

public sealed class CurrentAuditContext : IAuditContext
{
    private readonly IHttpContextAccessor _accessor;
    private readonly ICurrentUser _currentUser;

    public CurrentAuditContext(IHttpContextAccessor accessor, ICurrentUser currentUser)
    {
        _accessor = accessor;
        _currentUser = currentUser;
    }
    // Dar detalhamento sobre sobre as alteração ou log que são criados.
    public int? UserId => _currentUser.IsAuthenticated ? _currentUser.UserId : null;
    public string? CorrelationId => _accessor.HttpContext?.TraceIdentifier;
    public string? IpAddress => _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    public string? UserAgent => _accessor.HttpContext?.Request.Headers.UserAgent.ToString();
    public string? RequestPath => _accessor.HttpContext?.Request.Path.Value;
    public string? RequestMethod => _accessor.HttpContext?.Request.Method;

    // Sem ele: Senha alterada

    //Com ele, fica:
    //Usuário: 15
    //IP: 192.168.0.10
    //Rota: troca-senha
    //Método: POST
    //CorrelationId: abc123
    //UserAgent: Chrome
}
