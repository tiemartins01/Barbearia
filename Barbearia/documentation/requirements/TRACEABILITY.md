# Matriz de Rastreabilidade

| Requisito | Feature | Endpoint/Processo | Aplicação | Persistência/Infraestrutura |
|---|---|---|---|---|
| RF-AUTH-001 | FEAT-AUTH-001 | `POST /api/v1/auth/login` | `LoginService` | `LoginRepository` |
| RF-AUTH-002 | FEAT-AUTH-003 | `POST /api/v1/auth/refresh` | `RefreshTokenService` | `RefreshRepository` |
| RF-AUTH-003 | FEAT-AUTH-004 | `POST /api/v1/auth/logout` | `RefreshTokenService` | `RefreshRepository` |
| RF-PASSWORD-001 | FEAT-AUTH-005 | recuperação de senha | `EmailEsqueciSenhaService` | `EmailEsqueciSenhaRepository` |
| RF-PASSWORD-002 | FEAT-AUTH-006 | troca de senha | `TrocaSenhaService` | `TrocaSenhaRepository` |
| RF-USER-001 | FEAT-USER-001 | cadastro | `NovoClienteService` | `NovoClienteRepository` |
| RF-SERVICE-001 | FEAT-SERVICE-001 | `GET /api/v1/services` | `ServicosAtivosService` | `ServicosAtivosRepository` |
| RF-APPOINTMENT-001 | FEAT-APPOINTMENT-003 | `POST /api/v1/appointments` | `ProximoAtendimentoService` | `ProximoAtendimentoRepository` |
| RNF-CONSISTENCY-001 | FEAT-OPS-004 | criação concorrente | `ProximoAtendimentoService` | índice parcial único no PostgreSQL |
| RNF-RELIABILITY-001 | FEAT-OPS-001 | header `Idempotency-Key` | `IIdempotencyService` | `idempotency_records` |
| RNF-RELIABILITY-002 | FEAT-OPS-002 | `SaveChangesAsync` | Domain Events | `outbox_messages` |
| RNF-SCALABILITY-001 | FEAT-OPS-003 | worker em background | `OutboxProcessorService` | processamento em lote |
