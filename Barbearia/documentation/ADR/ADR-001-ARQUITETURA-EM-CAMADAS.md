# ADR-001 — Arquitetura em camadas

- **Status:** aceita no estado atual
- **Data registrada:** 2026-07-30

## Contexto

A aplicação precisa separar entrada HTTP, regras, persistência e testes.

## Decisão

Manter quatro projetos: API, Core, Infrastructure e Tests. A API compõe as dependências; o Core contém Domain/Application; Infrastructure implementa persistência.

## Consequências positivas

- reduz acoplamento direto entre controllers e EF Core;
- permite mocks de repositories;
- facilita evolução gradual;
- torna responsabilidades mais visíveis.

## Consequências negativas

- Core acumula responsabilidades de Domain e Application;
- organização por camada exige navegar por várias pastas para uma feature;
- convenções de namespace ainda são inconsistentes.

## Evolução esperada

Separar limites de Domain/Application e migrar gradualmente para organização por feature sem reescrever tudo de uma vez.
