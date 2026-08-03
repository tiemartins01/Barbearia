# Feature Status

## Fluxo FDD adotado

`Backlog → Em análise → Em desenvolvimento → Em revisão → Concluída`

| Feature | Prioridade | Versão | Status | Evidência principal |
|---|---:|---:|---|---|
| FEAT-APPOINTMENT-003 | Alta | v0.1 | Concluída | `ProximoAgendamentoController` e `ProximoAtendimentoService` |
| FEAT-OPS-001 | Alta | v0.4 | Concluída | `DatabaseIdempotencyService` |
| FEAT-OPS-002 | Alta | v0.4 | Concluída | `AppDbContext.AddDomainEventsToOutbox` |
| FEAT-OPS-003 | Alta | v0.4 | Concluída | `OutboxProcessorService` |
| FEAT-OPS-004 | Alta | v0.4 | Concluída | índice `ux_horarios_barbeiro_horario_ativo` |
| FEAT-OPS-005 | Média | v0.5 | Backlog | integração RabbitMQ/Kafka/Service Bus |
| FEAT-OPS-006 | Média | v0.5 | Backlog | Redis ou API Gateway |

## Critério de conclusão

Uma feature só deve ser marcada como concluída quando possuir:

1. requisito ou motivação registrada;
2. regra de negócio definida;
3. implementação integrada ao fluxo real;
4. documentação atualizada;
5. tratamento de falhas;
6. evidência de execução no ambiente local.
