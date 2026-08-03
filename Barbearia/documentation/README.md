# Documentação oficial — Barbearia Backend

**Versão documentada:** Backend Foundation v1.0  
**Base:** conteúdo do arquivo `Barbearia-corrigida(9).zip`  
**Escopo:** somente o backend existente no anexo.

Esta pasta descreve o estado real do backend no momento em que o documento foi escrito. Nenhum recurso futuro é apresentado como concluído. A documentação deve ser atualizada junto com cada alteração relevante do projeto.

## Visão geral

O backend é uma API ASP.NET Core 8 para uma barbearia. Ele oferece cadastro e autenticação de clientes, consulta e alteração de dados pessoais, listagem de barbeiros e serviços, consulta e marcação de horários, histórico, avaliações e recuperação/troca de senha.

## Tecnologias confirmadas

- .NET 8 e ASP.NET Core Web API;
- Entity Framework Core 8;
- PostgreSQL por Npgsql;
- autenticação JWT com access token e refresh token em cookies;
- proteção antiforgery/CSRF;
- FluentValidation;
- BCrypt;
- rate limiting;
- Swagger em ambiente Development;
- xUnit, Moq e Coverlet.

## Projetos da solução

```text
backend/
├── BarbeariaApi/             # HTTP, controllers, middleware e configuração
├── BarbeariaCore/            # Domain e Application
├── BarbeariaInfrastructure/  # EF Core, migrations e repositories
├── BarbeariaTests/           # testes unitários
└── Barbearia.sln
```

## Índice

- [Arquitetura](ARCHITECTURE.md)
- [Requisitos e situação atual](REQUIREMENTS.md)
- [API e endpoints](API.md)
- [Banco de dados](DATABASE.md)
- [Segurança](SECURITY.md)
- [Testes](TESTS.md)
- [Performance e escalabilidade](PERFORMANCE.md)
- [Troubleshooting](TROUBLESHOOTING.md)
- [Como executar](RUNBOOK.md)
- [Glossário do domínio](DOMAIN-GLOSSARY.md)
- [Changelog documental](CHANGELOG.md)
- [Roadmap técnico](ROADMAP.md)
- [Decisões arquiteturais](ADR/)
- [Diagramas em Mermaid](diagrams/)

## Regra de atualização

Uma mudança deve atualizar a documentação quando alterar ao menos um dos seguintes pontos:

- endpoint, request, response ou código HTTP;
- regra de negócio;
- entidade, relacionamento, índice ou migration;
- autenticação, autorização, CORS, CSRF ou rate limiting;
- dependência entre camadas;
- configuração necessária para execução;
- estratégia de testes;
- decisão arquitetural relevante.

## Limites desta versão

Nesta versão não há evidência de suporte implementado a SQL Server, MySQL, Redis, mensageria, microserviços, Kubernetes, CI/CD, métricas Prometheus, tracing distribuído ou testes de integração. Esses itens não devem ser considerados entregues.


## Objetivo 

Atingir os seguintes requisitos:

Práticas de DDD, FDD e TDD
Vivência em Scrum
Sólido conhecimento em React (hooks, context API, componentização)
Domínio de Git
Inglês intermediário
Análise de requisitos
Boas práticas de performance, escalabilidade e segurança
Integração de APIs REST
Troubleshooting avançado

## Modelo de domínio

- [Modelo de domínio — Fase 2](architecture/DOMAIN-MODEL.md)
- [Alterações da Fase 2](operations/PHASE-2-CHANGES.md)
