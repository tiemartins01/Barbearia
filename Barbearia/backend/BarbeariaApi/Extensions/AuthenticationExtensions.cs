using BarbeariaApi.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BarbeariaApi.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddBarbeariaAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Busca a chave utilizada para assinar e validar o JWT.
        // Essa chave deve ficar em User Secrets ou variável de ambiente.
        var jwtKey = configuration["Jwt:Key"];

        // Verifica se a chave existe e se possui tamanho mínimo de 32 bytes.
        // Uma chave curta deixa a assinatura do token mais vulnerável.
        if (string.IsNullOrWhiteSpace(jwtKey) ||
            Encoding.UTF8.GetByteCount(jwtKey) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Key deve possuir pelo menos 32 bytes.");
        }

        // Issuer identifica quem criou e assinou o token.
        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "Jwt:Issuer não configurado.");

        // Audience identifica para quem o token foi criado.
        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "Jwt:Audience não configurado.");

        /*
         * Define JWT Bearer como esquema padrão.
         *
         * Requisição
         *      ↓
         * UseAuthentication()
         *      ↓
         * JWT tenta localizar e validar o token
         *      ↓
         * Token inválido ou inexistente
         *      ↓
         * UseAuthorization()
         *      ↓
         * Endpoint exige [Authorize]
         *      ↓
         * DefaultChallengeScheme (JWT Bearer)
         *      ↓
         * Retorna 401 Unauthorized
         */
        services
            .AddAuthentication(options =>
            {
                // Usado pelo UseAuthentication() para tentar
                // identificar o usuário da requisição.
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                // Usado quando alguém tenta acessar um endpoint
                // protegido sem possuir autenticação válida.
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Define todas as regras que serão usadas
                // para validar o JWT recebido.
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        // Exige a validação da assinatura do token.
                        ValidateIssuerSigningKey = true,

                        // Cria a chave usada para conferir a assinatura.
                        // A mesma chave usada para gerar o token
                        // precisa ser usada para validá-lo.
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey)),

                        // Ativa a validação de quem emitiu o token.
                        ValidateIssuer = true,

                        // Define qual emissor é aceito.
                        ValidIssuer = issuer,

                        // Ativa a validação de para quem
                        // o token foi criado.
                        ValidateAudience = true,

                        // Define qual audiência é aceita.
                        ValidAudience = audience,

                        // Verifica se o token ainda está dentro
                        // do período de validade.
                        ValidateLifetime = true,

                        // Remove a tolerância padrão de alguns minutos.
                        // Assim que o token expira, ele já fica inválido.
                        ClockSkew = TimeSpan.Zero
                    };

                options.Events = new JwtBearerEvents
                {
                    /*
                     * Por padrão, JWT Bearer procura o token em:
                     *
                     * Authorization: Bearer TOKEN
                     *
                     * No seu projeto, o token está armazenado
                     * no cookie chamado "access-token".
                     */
                    OnMessageReceived = context =>
                    {
                        // Pega o JWT enviado automaticamente
                        // pelo navegador dentro do cookie.
                        context.Token =
                            context.Request.Cookies["access-token"];

                        return Task.CompletedTask;
                    }
                };
            });

        /*
         * Registra o handler personalizado da policy ActiveUser.
         *
         * ActiveUserRequirement
         *      ↓
         * ActiveUserHandler
         *      ↓
         * Verifica se o usuário autenticado continua ativo
         */
        services.AddScoped<
            IAuthorizationHandler,
            ActiveUserHandler>();

        // Configura as regras de autorização da aplicação.
        // Autenticação responde: "Quem é o usuário?"
        // Autorização responde: "O que ele pode acessar?"
        services.AddAuthorization(options =>
        {
            // Para acessar, o usuário precisa estar autenticado
            // e cumprir a regra personalizada ActiveUserRequirement.
            options.AddPolicy(
                "ActiveUser",
                policy => policy
                    .RequireAuthenticatedUser()
                    .AddRequirements(
                        new ActiveUserRequirement()));

            // Exige autenticação e a role Cliente.
            options.AddPolicy(
                "ClientOnly",
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireRole("Cliente"));

            // Exige autenticação e a role Barbeiro.
            options.AddPolicy(
                "BarberOnly",
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireRole("Barbeiro"));

            // Exige autenticação e a role Admin.
            options.AddPolicy(
                "AdminOnly",
                policy => policy
                    .RequireAuthenticatedUser()
                    .RequireRole("Admin"));
        });

        // Retorna a mesma coleção para permitir o encadeamento:
        //
        // builder.Services
        //     .AddBarbeariaAuthentication(...)
        //     .AddBarbeariaCors(...);
        return services;
    }
}