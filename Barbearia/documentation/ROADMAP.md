# Roadmap técnico sugerido

Este arquivo não representa funcionalidades concluídas. Ele registra a sequência recomendada de evolução a partir do estado atual.

## Etapa 1 — Base verificável

- ativar health checks;
- remover artefatos locais e secrets do versionamento;
- executar e estabilizar todos os testes;
- gerar cobertura;
- eliminar warnings relevantes;
- criar primeiro teste de integração.

## Etapa 2 — Robustez do agendamento

- garantir unicidade/concorrência no banco;
- tratar conflito de reserva;
- propagar `CancellationToken`;
- validar limites de paginação;
- adicionar testes simultâneos.

## Etapa 3 — Application e FDD

- separar Domain e Application em projetos ou limites claros;
- reorganizar gradualmente por features;
- começar por `Appointments`;
- adicionar Result Pattern e contratos consistentes;
- criar testes de arquitetura.

## Etapa 4 — API profissional

- adotar `ProblemDetails`;
- padronizar recursos e verbos REST;
- versionar API;
- enriquecer Swagger;
- documentar exemplos e códigos de erro.

## Etapa 5 — Observabilidade

- logging estruturado centralizado;
- métricas de latência, erros e banco;
- readiness e liveness;
- tracing distribuído;
- dashboards e alertas.

## Etapa 6 — Bancos exigidos pela vaga

- adicionar SQL Server e MySQL com configuração explícita;
- migrations independentes por provider;
- testes de integração para cada banco;
- documentação de diferenças e limitações.
