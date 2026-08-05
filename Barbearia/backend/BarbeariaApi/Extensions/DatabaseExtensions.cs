using Barbearia.BackgroundServices;
using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.Infrastructure.Data;
using Barbearia.Core.Infrastructure.Services;
using Barbearia.Core.Interface;
using Barbearia.Core.Repository;
using Barbearia.Core.Service;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace BarbeariaApi.Extensions
{
    public static class DatabaseExtensions
    {
        // AddBarbeariaDatabase:

        // Busca configuration.GetConnectionString("PostgreSql")
        //depois registra services.AddDbContext<AppDbContext>(...)

        // Caminho da conexão: 
        //Program.cs
        //    ↓
        //AddBarbeariaDatabase
        //    ↓
        //ConnectionStrings:PostgreSql
        //   ↓
        // AppDbContext
        //   ↓
        //Npgsql
        //    ↓
        //PostgreSQL

        // transforma o método em um método de extensão. -> this IServiceCollection

        // IConfiguration configuration -> Recebe as configurações carregadas pelo ASP.NET Core.
        // Exemplos: appsettings.json; appsettings.Development.json; User Secrets;
        public static IServiceCollection AddBarbeariaDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgreSql")
                ?? throw new InvalidOperationException("A connection string 'PostgreSql' não foi configurada.");
            // Atualmente só configurado para postgres
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Quantidade máxima de chamadas pós falha
                maxRetryDelay: TimeSpan.FromSeconds(10), // até 10 segundis entre chamadas
                errorCodesToAdd: null);
        });
            });

            return services;
        }
    }
}
