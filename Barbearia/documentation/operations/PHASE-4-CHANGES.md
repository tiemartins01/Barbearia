# Fase 4 — FDD, consistência e escalabilidade

## Alterações implementadas

### Idempotência de agendamento

`POST /api/v1/appointments` agora exige:

```http
Idempotency-Key: <valor único por tentativa lógica>
```

A mesma chave e o mesmo corpo retornam a resposta já armazenada. A resposta inclui:

```http
Idempotency-Replayed: true|false
```

A mesma chave com conteúdo diferente retorna conflito.

### Concorrência no banco

Foi criado o índice parcial único:

```text
ux_horarios_barbeiro_horario_ativo
```

Ele impede dois agendamentos com status `Agendado` para o mesmo barbeiro e horário. O conflito é convertido em `409 Conflict` com código `APPOINTMENT_TIME_CONFLICT`.

### Outbox Pattern

Os Domain Events dos Aggregate Roots são convertidos para registros em `outbox_messages` durante `SaveChangesAsync`.

### Processamento em background

`OutboxProcessorService` lê até 50 mensagens pendentes por lote, registra tentativas e marca mensagens concluídas. O ponto de integração com um broker externo ficou isolado para a próxima evolução.

### FDD

Foram consolidados:

- `features/FEATURE-LIST.md`;
- `features/FEATURE-STATUS.md`;
- `requirements/TRACEABILITY.md`;
- `ADR-006-IDEMPOTENCIA-E-OUTBOX.md`.

## Banco de dados

Esta fase exige migration:

```powershell
dotnet ef database update --project BarbeariaInfrastructure --startup-project BarbeariaApi
```

## Validação local

```powershell
dotnet restore
dotnet build
dotnet test
dotnet ef database update --project BarbeariaInfrastructure --startup-project BarbeariaApi
```

Depois, envie duas vezes o mesmo `POST /api/v1/appointments` com o mesmo corpo e a mesma `Idempotency-Key`. A segunda resposta deve conter `Idempotency-Replayed: true` e não deve criar um novo registro.
