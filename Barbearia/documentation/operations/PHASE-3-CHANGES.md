# Fase 3 — REST, Segurança, Performance e Troubleshooting

## Objetivo

Consolidar a borda HTTP e a operação do backend sem remover as rotas legadas usadas pela aplicação atual.

## Alterações implementadas

### API REST

- Adicionados aliases canônicos em `/api/v1`.
- As rotas anteriores foram preservadas para compatibilidade.
- Erros de domínio agora usam `application/problem+json`.
- O formato segue `ProblemDetails`, incluindo `status`, `detail`, `instance`, `errorCode` e `traceId`.
- Códigos de domínio passam a ser mapeados para HTTP 400, 401, 403, 404 e 409.
- Swagger recebeu título, versão e descrição da API.

### Segurança

- Rate limiting aplicado à recuperação de senha.
- Antiforgery aplicado às operações autenticadas que alteram estado:
  - logout;
  - refresh token;
  - alteração de dados;
  - criação de avaliação;
  - criação de agendamento.
- Login, cadastro e recuperação de senha permanecem explicitamente fora da validação antiforgery por serem fluxos anônimos.

### Performance

- Compressão HTTP habilitada, inclusive em HTTPS.
- Adicionado header `Server-Timing` com a duração da aplicação.
- Requisições acima de 750 ms geram warning estruturado.
- Instrumentação interna criada com `System.Diagnostics.Metrics`:
  - total de requisições;
  - erros 5xx;
  - duração;
  - requisições lentas.

### Troubleshooting

- Health Checks registrados e ativados:
  - `GET /health/live` — processo em execução;
  - `GET /health/ready` — prontidão e acesso ao PostgreSQL.
- Respostas dos Health Checks incluem status, duração e detalhes de cada verificação.
- Correlation ID e Trace ID continuam propagados nos logs e respostas.
- Erros internos não expõem stack trace, SQL ou detalhes de infraestrutura.

## Rotas REST adicionadas

| Operação | Rota canônica |
|---|---|
| Login | `POST /api/v1/auth/login` |
| Usuário autenticado | `GET /api/v1/auth/me` |
| Logout | `POST /api/v1/auth/logout` |
| Renovar sessão | `POST /api/v1/auth/refresh` |
| Cadastro | `POST /api/v1/users` |
| Consultar usuário | `GET /api/v1/users/me` |
| Alterar usuário | `PATCH /api/v1/users/me` |
| Recuperar senha | `POST /api/v1/password/recovery` |
| Redefinir senha | `POST /api/v1/password/reset` |
| Serviços | `GET /api/v1/services` |
| Barbeiros | `GET /api/v1/barbers` |
| Próximo agendamento | `GET /api/v1/appointments/next` |
| Horários disponíveis | `GET /api/v1/appointments/available-slots` |
| Criar agendamento | `POST /api/v1/appointments` |
| Histórico | `GET /api/v1/appointments/history` |
| Avaliação | `POST /api/v1/reviews` |
| Token CSRF | `GET /api/v1/security/csrf` |

## Compatibilidade

As rotas antigas continuam disponíveis nesta fase. A remoção deve ocorrer somente em uma versão futura, após migração dos consumidores.

## Banco de dados

Nenhuma tabela ou coluna foi alterada. Não é necessária nova migration.

## Validação necessária no ambiente local

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project backend/BarbeariaApi
```

Depois, validar:

```text
GET /health/live
GET /health/ready
GET /swagger
```
