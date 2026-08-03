# Feature List

## Convenção

- `FEAT-AUTH`: autenticação e sessão.
- `FEAT-USER`: cadastro e dados pessoais.
- `FEAT-SERVICE`: serviços da barbearia.
- `FEAT-APPOINTMENT`: agendamentos.
- `FEAT-REVIEW`: avaliações.
- `FEAT-OPS`: capacidades operacionais.

## Funcionalidades

| ID | Funcionalidade | Situação |
|---|---|---|
| FEAT-AUTH-001 | Autenticar usuário | Concluída |
| FEAT-AUTH-002 | Consultar usuário autenticado | Concluída |
| FEAT-AUTH-003 | Renovar sessão com refresh token | Concluída |
| FEAT-AUTH-004 | Encerrar sessão | Concluída |
| FEAT-AUTH-005 | Solicitar recuperação de senha | Concluída |
| FEAT-AUTH-006 | Alterar senha com código | Concluída |
| FEAT-USER-001 | Cadastrar cliente | Concluída |
| FEAT-USER-002 | Consultar dados pessoais | Concluída |
| FEAT-USER-003 | Alterar dados pessoais | Concluída |
| FEAT-SERVICE-001 | Listar serviços ativos | Concluída |
| FEAT-APPOINTMENT-001 | Consultar barbeiros | Concluída |
| FEAT-APPOINTMENT-002 | Consultar horários disponíveis | Concluída |
| FEAT-APPOINTMENT-003 | Criar agendamento | Concluída |
| FEAT-APPOINTMENT-004 | Consultar próximo agendamento | Concluída |
| FEAT-APPOINTMENT-005 | Consultar histórico | Concluída |
| FEAT-REVIEW-001 | Registrar avaliação | Concluída |
| FEAT-OPS-001 | Impedir duplicidade de criação por Idempotency Key | Concluída na Fase 4 |
| FEAT-OPS-002 | Persistir Domain Events pelo Outbox Pattern | Concluída na Fase 4 |
| FEAT-OPS-003 | Processar eventos do Outbox em background | Concluída na Fase 4 |
| FEAT-OPS-004 | Impedir dois agendamentos ativos no mesmo horário | Concluída na Fase 4 |
| FEAT-OPS-005 | Publicar eventos em broker externo | Planejada |
| FEAT-OPS-006 | Cache e rate limit distribuídos | Planejada |
