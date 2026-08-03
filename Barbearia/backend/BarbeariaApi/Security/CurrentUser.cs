using System.Security.Claims;
using Barbearia.Core.Application.Abstractions;

namespace BarbeariaApi.Security;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public int UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(value, out var userId))
            {
                return userId;
            }

            return 0;
        }
    }

    public string Name =>
        User?.FindFirstValue(ClaimTypes.Name)
        ?? string.Empty;

    public string Role =>
        User?.FindFirstValue(ClaimTypes.Role)
        ?? string.Empty;
}