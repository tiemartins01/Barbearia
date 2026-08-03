# ADR-005 — PostgreSQL como provider atual

- **Status:** aceita no estado atual
- **Data registrada:** 2026-07-30

## Contexto

A aplicação necessita persistência relacional com EF Core.

## Decisão

Usar PostgreSQL com Npgsql e uma migration inicial. Habilitar retry para falhas transitórias.

## Consequências positivas

- provider maduro para EF Core;
- suporte relacional completo;
- migrations versionadas;
- retry ajuda em falhas transitórias.

## Consequências negativas

- esta versão não atende SQL Server e MySQL;
- comportamento específico de provider precisa de testes reais;
- retry não substitui health checks ou tratamento de indisponibilidade.
