# ERROS JÁ CORRIGIDOS NESSA VERSÃO

Este arquivo representa correções concluídas já nessa feature.

## Erro 1 — Troca de senha sem validar a senha antiga

if (!usuario.Senha.Verify(dados.SenhaAntiga)
    && !string.IsNullOrEmpty(dados.SenhaAntiga))
{
    throw new DomainException("Credenciais inválidas!");
}

Para dar erro, precisa que a senha antiga estivesse errada ou a o campo estar vazio.

Contudo, no código que estava, sempre ia dar erro.

Correto:

if (string.IsNullOrWhiteSpace(dados.SenhaAntiga) ||
    !usuario.Senha.Verify(dados.SenhaAntiga))
{
    throw new DomainException(
        "AUTH_INVALID_CURRENT_PASSWORD",
        "A senha atual é obrigatória e deve estar correta.");
}

## Erro 2 — Exposição das senhas no retorno da API

Endpoint retornava return Ok(request); e no DTO contém a nova senha e a antiga.

Com isso, o retorno HTTP devolvia informações sensíveis para o cliente.

Correto:

return NoContent();

## Erro 3 — Refresh token não era enviado ao logout

O refresh cookie estava configurado com:

Path = "/login/refresh"

Cookies só são enviados para caminhos compatíveis com seu Path.

Assim, o navegador enviava o refresh token para:

/login/refresh

mas não para:

/login/logout

Consequentemente, o logout apagava o cookie do navegador, mas normalmente não conseguia recuperar e revogar o token no banco.

Foi corrigido para:

Path = "/login"

Agora o cookie pode ser enviado tanto ao refresh quanto ao logout.

## Erro 4 — Paginação sem limites

Era possível solicitar valores como: ?page=-50&pageSize=1000000

Foi incluido no endpoint do histórico :

if (page < 1 || pageSize is < 1 or > 100)
{
    return BadRequest(...);
}

Isso evita parâmetros inválidos e consultas excessivamente grandes.

## Erro 5 — Expiração dos cookies

Alteração de DateTime.Now -> DateTimeOffset.UtcNow

Evitar problemas de banco

## Erro 6 — Corrigir a porta da API no frontend

Backend inicia em http://localhost:5244 -> definido em backend/BarbeariaApi/Properties/launchSettings.json

No perfil HTTP:

"applicationUrl": "http://localhost:5244"

O arquivo:

frontend/.env

já contém:

VITE_API_URL=http://localhost:5244

Porém, o arquivo:

frontend/.env.example

ainda contém:

VITE_API_URL=http://localhost:5077
Alteração

Abra:

frontend/.env.example

Troque para:

VITE_API_URL=http://localhost:5244

Também recomendo usar:

frontend/.env.local

com:

VITE_API_URL=http://localhost:5244

E remover o frontend/.env do Git.

## Melhoria — Criar o PostgreSQL com Docker


Uso de volume -> Sem volume, os dados podem desaparecer quando o container for removido. (PORTA 5433 porque a 5432 o postgres local estava usando)


## Erro 7 — SMPT obrigatório

Na classe SmtpSettings foi inserido o campo public bool Enabled { get; set; }

Em backend/BarbeariaApi/Extensions/ServiceCollectionExtensions.cs foi relaizado a verificação 

public static IServiceCollection AddBarbeariaEmail(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services
        .AddOptions<SmtpSettings>()
        .Bind(configuration.GetSection("SmtpSettings"))
        .Validate(
            settings =>
                !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.Host),
            "SmtpSettings:Host não configurado.")
        .Validate(
            settings =>
                !settings.Enabled ||
                settings.Port > 0,
            "SmtpSettings:Port inválido.")
        .Validate(
            settings =>
                !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.FromEmail),
            "SmtpSettings:FromEmail não configurado.")
        .Validate(
            settings =>
                !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.Username),
            "SmtpSettings:Username não configurado.")
        .Validate(
            settings =>
                !settings.Enabled ||
                !string.IsNullOrWhiteSpace(settings.Password),
            "SmtpSettings:Password não configurado.")
        .ValidateOnStart();

    services.AddScoped<IEnviarEmail, EnviarEmail>();

    return services;
}

Na classe EnviarEmail não é mais usado configuration, e sim  private readonly SmtpSettings _settings; 

if (!_settings.Enabled)
        {
            throw new InvalidOperationException(
                "O envio de e-mail não está habilitado.");
        }


## Erro 8 - Estrutura incorreta

EnviarEmail não deveria permanecer em BarberShop.Core.Service.

Porque SMPT é infraestrutura, então foi preciso pegar do core e jogar BarbeariaInfrastructure, até porque quando estava no core e ele não depende de ninguém, quando ia declarar, apresentava erro.

## Erro 9 - Corrigir cookies em HTTP local

Código atual:

Secure = true;
SameSite = SameSiteMode.None;

O backend local roda em:

http://localhost:5244

Cookies Secure devem ser enviados apenas por HTTPS. Em HTTP local, o navegador pode descartar ou deixar de enviar o cookie.

Problema adicional?

Metodos atuais são static, não sendo possível consulta Request.IsHttps

private static CookieOptions CookieOptionsAccess()
private static CookieOptions CookieOptionsRefresh()

Alteração foi retirar o static e alterar os metodos

Cookie de acesso

Use:

private CookieOptions CookieOptionsAccess()
{
    var isHttps = Request.IsHttps;

    return new CookieOptions
    {
        HttpOnly = true,
        Secure = isHttps,
        SameSite = isHttps
            ? SameSiteMode.None
            : SameSiteMode.Lax,
        IsEssential = true,
        Expires = DateTimeOffset.UtcNow.Add(AccessExpiration),
        Path = "/"
    };
}
Cookie de refresh

Use:

private CookieOptions CookieOptionsRefresh()
{
    var isHttps = Request.IsHttps;

    return new CookieOptions
    {
        HttpOnly = true,
        Secure = isHttps,
        SameSite = isHttps
            ? SameSiteMode.None
            : SameSiteMode.Lax,
        IsEssential = true,
        Expires = DateTimeOffset.UtcNow.Add(RefreshExpiration),
        Path = "/login" -> ALTERADO TAMBÉM
    };
}

## Erro 10 - Permitir logout com token de acesso expirado

O middleware pode alegar erro de autenticação e retornar 401, sendo que está expirado.

Troca feita de [Authorize] para [AllowAnonymous]

Além de melhorar o tratamento da exceção

catch (Exception exception)
{
    _logger.LogWarning(
        exception,
        "Não foi possível revogar o refresh token durante o logout.");
}

E precisou colocar ILogger<LoginController> no construtor








































