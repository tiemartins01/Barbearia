using Barbearia.BackgroundServices;
using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.Infrastructure.Services;
using Barbearia.Core.Interface;
using Barbearia.Core.Repository;
using Barbearia.Core.Service;
using BarbeariaCore.Application.Interfaces;
using BarbeariaInfrastructure.Security;

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
