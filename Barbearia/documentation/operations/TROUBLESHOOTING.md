# Guia de troubleshooting

## API não inicia

Verifique, nesta ordem:

1. SDK .NET 8 instalado: `dotnet --info`;
2. restore: `dotnet restore`;
3. `ConnectionStrings:PostgreSql` configurada;
4. `Jwt:Key`, `Jwt:Issuer` e `Jwt:Audience` configurados;
5. chave JWT com pelo menos 32 bytes;
6. configurações SMTP válidas quando `SmtpSettings:Enabled` for verdadeiro;
7. PostgreSQL acessível.

A aplicação falha durante o startup quando connection string ou JWT obrigatório estão ausentes.

## Banco não conecta

- confirme host, porta, database, usuário e senha;
- confirme se o container/serviço PostgreSQL está ativo;
- rode `dotnet ef database update`;
- confira firewall e porta;
- procure falhas após as tentativas de retry do Npgsql.

## Login retorna 400

Um `DomainException` é convertido para 400. Use o campo `codigo` e o `traceId` da resposta. Verifique:

- formato do request (`Nome` e `Senha`);
- existência e estado do usuário;
- hash BCrypt;
- bloqueio por tentativas;
- logs com o mesmo correlation ID.

## Login retorna 401

- confirme se o cookie `access-token` foi gravado;
- confirme `withCredentials: true` no frontend;
- valide origem CORS;
- confira issuer, audience e expiração;
- confirme que o relógio da máquina está correto, pois `ClockSkew` é zero.

## Requisição POST retorna erro antiforgery

1. chame `GET /csrf`;
2. preserve o cookie `XSRF-TOKEN`;
3. envie o token retornado no header `X-CSRF-TOKEN`;
4. envie cookies com credenciais;
5. confirme protocolo HTTP/HTTPS e SameSite.

O endpoint `POST /login` é a exceção e ignora antiforgery.

## CORS ou cookies não funcionam

- confirme a origem configurada em `AllowReact`;
- não misture `localhost` com `127.0.0.1`;
- confirme porta e protocolo;
- use `withCredentials: true`;
- em HTTPS, os cookies usam SameSite None e Secure.

## Rate limit — HTTP 429

Aguarde o fim da janela correspondente. Confira a policy aplicada no endpoint e o limite definido em `AddRateLimiting`.

## Erro 500

A resposta esconde detalhes internos e fornece `traceId`. Procure nos logs:

```text
Erro não tratado em {Metodo} {Path}. TraceId: {TraceId}
```

Nunca exponha stack trace ao cliente em produção.

## Migration falha

Na pasta `backend`:

```bash
dotnet ef database update --project BarbeariaInfrastructure --startup-project BarbeariaApi
```

Se o comando `dotnet ef` não existir:

```bash
dotnet tool install --global dotnet-ef --version 8.*
```

## Health check

As classes de health check existem, mas o endpoint não está ativo nesta versão. Portanto, tentar acessar `/health` não prova falha do banco; essa rota ainda não foi mapeada.

## Uso do correlation ID

O cliente pode enviar `X-Correlation-ID`. Quando ausente, a API gera um GUID. O mesmo valor volta no header de resposta e é usado como `TraceIdentifier`, permitindo localizar a requisição nos logs.
