# ADR-006 — Idempotência e Outbox

## Status

Aceito na Fase 4.

## Contexto

A criação de agendamento pode ser repetida por timeout, duplo clique ou retry do cliente. Além disso, salvar uma alteração e publicar um evento em operações separadas permite perda de mensagens.

## Decisão

1. Exigir `Idempotency-Key` em `POST /api/v1/appointments`.
2. Persistir chave, hash da requisição e resposta no PostgreSQL.
3. Impedir a reutilização da mesma chave com conteúdo diferente.
4. Salvar Domain Events em `outbox_messages` na mesma unidade de persistência das alterações do domínio.
5. Processar o Outbox em lotes por um `BackgroundService`.
6. Manter um ponto explícito para futura integração com RabbitMQ, Kafka ou Service Bus.

## Consequências positivas

- retries não criam agendamentos duplicados;
- eventos não dependem da disponibilidade imediata de um broker;
- processamento em background reduz acoplamento;
- a base fica preparada para múltiplas instâncias.

## Consequências negativas

- duas novas tabelas operacionais;
- necessidade de limpeza de registros expirados;
- o worker atual publica em log e ainda não entrega a um broker externo;
- é necessário aplicar a migration da Fase 4.
