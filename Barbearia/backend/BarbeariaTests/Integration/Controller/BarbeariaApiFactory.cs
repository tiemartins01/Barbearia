using Barbearia.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BarbeariaTests.Integration.Controller;

public sealed class BarbeariaApiFactory : WebApplicationFactory<Program>
{
    private const string JwtKey =
        "barbearia-integration-tests-jwt-key-2026-com-mais-de-32-bytes";

    private readonly string _connectionString;

    public BarbeariaApiFactory(string connectionString)
    {
        _connectionString = connectionString;

        // O Program valida o JWT durante a inicialização da aplicação.
        // As variáveis precisam existir antes de CreateClient iniciar o host.
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__PostgreSql",
            _connectionString);

        Environment.SetEnvironmentVariable("Jwt__Key", JwtKey);
        Environment.SetEnvironmentVariable("Jwt__Issuer", "BarbeariaTests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "BarbeariaTests");
        Environment.SetEnvironmentVariable("SmtpSettings__Enabled", "false");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Mantém também uma fonte em memória para garantir que os valores
        // de teste tenham prioridade sobre o appsettings.json vazio.
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:PostgreSql"] = _connectionString,
                    ["Jwt:Key"] = JwtKey,
                    ["Jwt:Issuer"] = "BarbeariaTests",
                    ["Jwt:Audience"] = "BarbeariaTests",
                    ["Frontend:Url"] = "http://localhost:5173",
                    ["SmtpSettings:Enabled"] = "false"
                });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString));

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        Environment.SetEnvironmentVariable("ConnectionStrings__PostgreSql", null);
        Environment.SetEnvironmentVariable("Jwt__Key", null);
        Environment.SetEnvironmentVariable("Jwt__Issuer", null);
        Environment.SetEnvironmentVariable("Jwt__Audience", null);
        Environment.SetEnvironmentVariable("SmtpSettings__Enabled", null);
    }
}
