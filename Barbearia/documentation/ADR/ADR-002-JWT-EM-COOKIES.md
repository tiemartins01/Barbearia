# ADR-002 — JWT em cookies HttpOnly

- **Status:** aceita no estado atual
- **Data registrada:** 2026-07-30

## Contexto

A aplicação web precisa manter sessão autenticada entre frontend e API.

## Decisão

Armazenar access token e refresh token em cookies HttpOnly. O JWT Bearer lê o access token do cookie. Requisições mutáveis são protegidas por antiforgery.

## Consequências positivas

- JavaScript não lê diretamente os tokens HttpOnly;
- refresh token pode ter path restrito a `/login`;
- autenticação integra-se ao pipeline padrão do ASP.NET Core.

## Consequências negativas

- exige CORS com credenciais e configuração correta de SameSite;
- exige CSRF para operações mutáveis;
- diferenças entre HTTP local e HTTPS precisam ser testadas.
