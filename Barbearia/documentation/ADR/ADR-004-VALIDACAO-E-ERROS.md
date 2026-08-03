# ADR-004 — FluentValidation e erro global

- **Status:** aceita no estado atual
- **Data registrada:** 2026-07-30

## Contexto

Requests inválidos e violações de domínio precisam de respostas previsíveis e seguras.

## Decisão

Usar validadores FluentValidation para DTOs e `DomainException` para regras. O `ErrorHandlingMiddleware` converte exceções de domínio em 400 e erros inesperados em 500 genérico com trace ID.

## Consequências positivas

- separa validação de entrada da regra de negócio;
- evita exposição de detalhes internos;
- permite rastrear erros pelo trace ID;
- fornece códigos de domínio ao cliente.

## Consequências negativas

- validadores precisam estar corretamente registrados;
- diferenças entre erro de validação e erro de domínio precisam ser documentadas;
- o filtro customizado existe, mas seu registro efetivo deve ser sempre conferido.
