using Google.Protobuf.WellKnownTypes;

namespace BarbeariaApi.Extensions;
public static class CorsExtensions // Diz ao navegador quais sites podem acessar sua API.
{ // O CORS controla quais frontends podem acessar sua API pelo navegador.
    public static IServiceCollection AddBarbeariaCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var frontendUrl =
            configuration["Frontend:Url"]
            ?? "http://localhost:5173";
        // Verifica -> Origem permitida? Método permitido? Header pode ser enviado? Pode enviar cookies?
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReact", policy =>
            {
                policy
                    .WithOrigins(frontendUrl) // Permite somente essa origem
                    .WithMethods(
                        "GET",
                        "POST",
                        "PUT",
                        "PATCH",
                        "DELETE")
                    //"Frontend, você pode enviar um header chamado X-CSRF-TOKEN."
                    .WithHeaders( // Permite que o frontend envie esses headers.
                            "Content-Type", // Content-Type: application/json
                            "X-CSRF-TOKEN", //Transporta o token antiforgery.
                            "Idempotency-Key", // Identifica requisições que não devem ser processadas duas vezes.
                            "traceparent", // Transporta informações de trace distribuído.
                            "tracestate") // Transporta estado adicional do trace.
                        .AllowCredentials(); // Permite envio de credenciais, principalmente cookies 
                                             // precisa do AllowCredentials devido uso de cookies
            });
        });

        return services;
    }
}