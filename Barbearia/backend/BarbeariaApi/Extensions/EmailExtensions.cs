using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Services;
using BarbeariaInfrastructure;

namespace BarbeariaApi.Extensions
{
    public static class EmailExtensions
    {

        public static IServiceCollection AddBarbeariaEmail(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            services
        .AddOptions<SmtpSettings>()
        .Bind(configuration.GetSection("SmtpSettings"))
        .Validate(settings =>
                !settings.Enabled || !string.IsNullOrWhiteSpace(settings.Host),
            "SmtpSettings:Host não configurado.")
        .Validate(settings => !settings.Enabled ||
                settings.Port > 0,
            "SmtpSettings:Port inválido.")
        .Validate(settings => !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.FromEmail),
            "SmtpSettings:FromEmail não configurado.")
        .Validate(settings => !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.Username),
            "SmtpSettings:Username não configurado.")
        .Validate(settings => !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.Password),
            "SmtpSettings:Password não configurado.")
        .ValidateOnStart();

            services.AddScoped<IEnviarEmail, EnviarEmail>();

            return services;
        }

    }
}
