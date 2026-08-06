using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaApi.Security;
// Implementa a regra do usuário ativo

//JWT válido
//→ HttpContext.User preenchido
//→ CurrentUser lê as claims
//→ ActiveUserHandler consulta o banco
//→ usuário existe e Ativado = true ?
//→ sim: autorização aprovada
//→ não: autorização negada

public sealed class ActiveUserHandler : AuthorizationHandler<ActiveUserRequirement>
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _currentUser;

    public ActiveUserHandler(AppDbContext db, ICurrentUser currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }
    // Faz a verificação/autenticação do usuário
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveUserRequirement requirement)
    {
        if (!_currentUser.IsAuthenticated)
            return;

        var active = await _db.Usuarios.AsNoTracking()
            .AnyAsync(x => x.Id == _currentUser.UserId && x.Ativado);

        if (active)
            context.Succeed(requirement);
    }
}
