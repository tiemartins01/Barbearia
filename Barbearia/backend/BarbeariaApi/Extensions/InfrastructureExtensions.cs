using Barbearia.BackgroundServices;
using BarbeariaCore.Application.Abstractions;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaInfrastructure.Queries;
using BarbeariaInfrastructure.Repository;
using BarbeariaInfrastructure.Security;
using BarbeariaInfrastructure.Services;

namespace BarbeariaApi.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddBarbeariaInfrastructure(
            this IServiceCollection services)
        {
            // Aggregate repositories
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
            services.AddScoped<IBarbeiroRepository, BarbeiroRepository>();
            services.AddScoped<IServicoRepository, ServicoRepository>();
            services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();

            // Queries
            services.AddScoped<IBarbeirosQuery, BarbeirosQuery>();
            services.AddScoped<IHistoricoClienteQuery, HistoricoClienteQuery>();
            services.AddScoped<IDadosPessoaisQuery, DadosPessoaisQuery>();
            services.AddScoped<IProximoAgendamentoQuery, ProximoAgendamentoQuery>();
            services.AddScoped<IServicosAtivosQuery, ServicosAtivosQuery>();
            services.AddScoped<IAgendaDisponibilidadeQuery, AgendaDisponibilidadeQuery>();

            // Technical infrastructure
            services.AddScoped<IRefreshRepository, RefreshRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorksRepository>();
            services.AddScoped<IPasswordHash, PasswordHasher>();
            services.AddScoped<IIdempotencyService, DatabaseIdempotencyService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddHostedService<OutboxProcessorService>();

            return services;
        }
    }
}
