using Barbearia.Core.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Barbearia.HealthChecks;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(AppDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("Conexão com o banco de dados estabelecida.")
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar a saúde do banco de dados.");
            return HealthCheckResult.Unhealthy("Erro ao acessar o banco de dados.");
        }
    }
}
