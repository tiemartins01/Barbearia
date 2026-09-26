# Roadmap técnico sugerido

Este arquivo não representa funcionalidades concluídas. Ele registra a sequência recomendada de evolução a partir do estado atual.

## Etapa 1 — Base verificável

- ativar health checks; Concluido.
- remover artefatos locais e secrets do versionamento; Concluido.
- executar e estabilizar todos os testes; Em processo final, necessidade de testar novamente.
- gerar cobertura;
- eliminar warnings relevantes; - Ainda não completo
- criar primeiro teste de integração.

## Etapa 2 — Robustez do agendamento

- garantir unicidade/concorrência no banco; - Ainda não completo
- tratar conflito de reserva; - Ainda não completo
- propagar `CancellationToken`; - Concluído.
- validar limites de paginação; - Concluído.
- adicionar testes simultâneos. - Em processo.

## Etapa 3 — Application e FDD

- separar Domain e Application em projetos ou limites claros; - Em processo
- reorganizar gradualmente por features; - Ainda não completo.
- começar por `Appointments`; - Não iniciado
- adicionar Result Pattern e contratos consistentes; - Não iniciado
- criar testes de arquitetura. - Necessário testar

## Etapa 4 — API profissional

- adotar `ProblemDetails`; - ainda não feito.
- padronizar recursos e verbos REST; - Concluído.
- versionar API; - Necessário avaliar.
- enriquecer Swagger; - Avaliar necessidade
- documentar exemplos e códigos de erro. - Realizando.

## Etapa 5 — Observabilidade

Todos ainda são preciso avaliar.

- logging estruturado centralizado;
- métricas de latência, erros e banco;
- readiness e liveness;
- tracing distribuído;
- dashboards e alertas.

## Etapa 6 — Bancos exigidos pela vaga

Iniciado mas ainda não testado

- adicionar SQL Server e MySQL com configuração explícita;
- migrations independentes por provider;
- testes de integração para cada banco;
- documentação de diferenças e limitações.
