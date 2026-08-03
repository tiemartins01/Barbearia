# Segurança

## Autenticação

A aplicação usa JWT Bearer. O token não é recebido pelo header `Authorization`; ele é extraído do cookie `access-token` no evento `OnMessageReceived`.

Parâmetros validados:

- assinatura;
- issuer;
- audience;
- expiração;
- `ClockSkew = TimeSpan.Zero`.

A chave JWT deve possuir no mínimo 32 bytes.

## Cookies

### Access token

- nome: `access-token`;
- HttpOnly: sim;
- Secure: acompanha `Request.IsHttps`;
- SameSite: `None` em HTTPS e `Lax` em HTTP;
- validade configurada no controller: 15 minutos;
- path: `/`.

### Refresh token

- nome: `refresh-token`;
- HttpOnly: sim;
- validade: 7 dias;
- path: `/login`;
- demais propriedades acompanham o access token.

## Autorização

Os endpoints da área do cliente usam:

```csharp
[Authorize(Roles = "Cliente")]
```

O endpoint `/login/me` exige apenas autenticação.

## CSRF

A aplicação registra `AutoValidateAntiforgeryTokenAttribute` globalmente. O token é obtido em `GET /csrf` e enviado no header `X-CSRF-TOKEN`.

O cookie antiforgery se chama `XSRF-TOKEN`, não é HttpOnly e usa SameSite Lax. Isso é esperado para o padrão double-submit, pois o frontend precisa ler ou receber o request token.

O login ignora antiforgery explicitamente. Essa é uma decisão existente e deve permanecer documentada e testada.

## Senhas

O projeto usa `BCrypt.Net-Next` para hash e verificação de senha. Value objects e serviços aplicam validações adicionais.

## Rate limiting

| Policy | Limite atual |
|---|---:|
| login | 5/minuto |
| cadastro | 5/5 minutos |
| recuperação de senha | 3/10 minutos |
| troca de senha | 10/10 minutos |
| refresh | 20/minuto |

A policy de recuperação de senha foi registrada, mas não está aplicada no controller de envio de e-mail. 

## Headers de segurança

O middleware adiciona:

- `X-Content-Type-Options: nosniff`;
- `X-Frame-Options: DENY`;
- `Referrer-Policy: strict-origin-when-cross-origin`;
- `Permissions-Policy` bloqueando câmera, microfone e geolocalização.

## CORS

Existe uma policy chamada `AllowReact`. A origem permitida é lida da configuração da aplicação. O frontend deve usar credenciais para enviar cookies.

## Gestão de segredos

O projeto possui `UserSecretsId`, `.env.example` e `appsettings.Development.example.json`. Segredos reais não devem ser commitados. Use User Secrets ou variáveis de ambiente para:

- connection string;
- chave JWT;
- credenciais SMTP.

## Riscos e pendências

- criar testes automatizados de autenticação, cookies e CSRF;
- aplicar rate limit ao endpoint de recuperação;
- revisar se logout anônimo é a política desejada;
- confirmar proteção de concorrência de horários no banco;
- remover secrets reais e artefatos locais do versionamento;
- adicionar política de rotação e revogação observável de refresh tokens.
