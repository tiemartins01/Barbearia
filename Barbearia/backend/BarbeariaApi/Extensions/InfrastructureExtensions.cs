using Barbearia.BackgroundServices;
using BarbeariaCore.Application.Abstractions;
using BarbeariaCore.Application.Services;
using BarbeariaCore.Application.Interfaces;
using BarbeariaInfrastructure.Repository;
using BarbeariaInfrastructure.Security;
using BarbeariaInfrastructure.Services;

namespace BarbeariaApi.Extensions
{
    public static class InfrastructureExtensions
    {

        public static IServiceCollection AddBarbeariaInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IAbaClienteRepository, AbaClienteRepository>();
            services.AddScoped<IServicosRepository, ServicosAtivosRepository>();
            services.AddScoped<IEmailEsqueciSenhaRepository, EmailEsqueciSenhaRepository>();
            services.AddScoped<INovoClienteRepository, NovoClienteRepository>();
            services.AddScoped<IProximoAtendimentoRepository, ProximoAtendimentoRepository>();
            services.AddScoped<ITrocaSenhaRepository, TrocaSenhaRepository>();
            services.AddScoped<IRefreshRepository, RefreshRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWorksRepository>();
            services.AddScoped<IPasswordHash, PasswordHasher>();
            services.AddScoped<IIdempotencyService, DatabaseIdempotencyService>();
            services.AddHostedService<OutboxProcessorService>(); //roda em segundo plano
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

    }
}
