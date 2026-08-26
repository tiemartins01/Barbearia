using BarbeariaCore.Application.Interfaces.Services;
using BarbeariaCore.Application.Services;

namespace BarbeariaApi.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddBarbeariaApplication(
            this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IAbaClienteService, AbaClienteService>();
            services.AddScoped<IServicosService, ServicosAtivosService>();
            services.AddScoped<IEmailEsqueciSenhaService, EmailEsqueciSenhaService>();
            services.AddScoped<INovoClienteService, NovoClienteService>();
            services.AddScoped<IProximoAtendimentoService, ProximoAtendimentoService>();
            services.AddScoped<ITrocaSenhaService, TrocaSenhaService>();

            // IRefreshTokenService permanece como já existe no seu projeto.
            services.AddScoped<
                BarbeariaCore.Application.Interfaces.IRefreshTokenService,
                RefreshTokenService>();

            return services;
        }
    }
}
