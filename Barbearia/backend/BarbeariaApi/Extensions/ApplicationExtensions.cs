using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Services;

namespace BarbeariaApi.Extensions
{
    public static class ApplicationExtensions
    {

        // Application regras dos casos de uso orquestra o fluxo usa interfaces
        // Registro de serviços do Core
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
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            return services;
        }
    }
}
