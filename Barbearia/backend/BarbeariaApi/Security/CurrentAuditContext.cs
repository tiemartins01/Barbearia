using Barbearia.Core.Application.Abstractions;

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

    public int? UserId => _currentUser.IsAuthenticated ? _currentUser.UserId : null;
    public string? CorrelationId => _accessor.HttpContext?.TraceIdentifier;
    public string? IpAddress => _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    public string? UserAgent => _accessor.HttpContext?.Request.Headers.UserAgent.ToString();
    public string? RequestPath => _accessor.HttpContext?.Request.Path.Value;
    public string? RequestMethod => _accessor.HttpContext?.Request.Method;
}
