using BarbeariaCore.Application.Abstractions;
using BarbeariaApi.Security;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaApi.Extensions;

public static class ApiServicesExtensions
{
    public static IServiceCollection AddBarbeariaApiServices(
        this IServiceCollection services)
    {
        services.AddControllersWithViews( options =>
        {
            options.Filters.Add(
                new AutoValidateAntiforgeryTokenAttribute()); // Faz a validação do anti-forgery por comparação
        });
        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Barbearia API",
                    Version = "v1",
                    Description =
                        "API REST para autenticação, clientes, serviços e agendamentos."
                });
        });

        services.AddHttpContextAccessor(); // Permite que o HttpContext seja acessado fora dos controllers

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "XSRF-TOKEN";
            options.Cookie.HttpOnly = false;

            var isProduction = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                "Production",
                StringComparison.OrdinalIgnoreCase);

            options.Cookie.SecurePolicy = isProduction ? 
                CookieSecurePolicy.Always :
                CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = isProduction ?
                SameSiteMode.None :
                SameSiteMode.Lax;
        });

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAuditContext, CurrentAuditContext>();

        return services;
    }
}