# ADR-003 — Repositories e Unit of Work

- **Status:** aceita no estado atual
- **Data registrada:** 2026-07-30

## Contexto

Services não devem depender diretamente dos detalhes de consultas e persistência do EF Core.

## Decisão

Definir interfaces no Core e implementações concretas na Infrastructure. Usar `IUnitOfWorks` para confirmar alterações.

## Consequências positivas

- services podem ser testados com Moq;
- persistência fica concentrada na Infrastructure;
- regras de aplicação não precisam conhecer todas as APIs do EF Core.

## Consequências negativas

- aumenta o número de interfaces e arquivos;
- repositories muito específicos podem duplicar capacidades do DbContext;
- exige testes de integração para provar que as implementações funcionam.
