# Arquitetura atual

## Estilo predominante

O backend utiliza uma arquitetura em camadas, separada em quatro projetos. Há elementos de DDD no domínio e separação entre contratos e implementações, mas atualmente a organização ainda não é integralmente orientada por features. 

## Responsabilidades

### BarbeariaApi

Responsável pela fronteira HTTP:

- controllers e rotas;
- autenticação e autorização;
- configuração de CORS e CSRF;
- Swagger;
- rate limiting;
- middlewares de erro, logging e headers;
- composição das dependências no `ServiceCollectionExtensions`;
- implementação de `CurrentUser`.

### BarbeariaCore

Contém duas áreas principais:

- **Domain:** entidades, enums, value objects e `DomainException`;
- **Application:** DTOs, interfaces, services, validadores e contratos de infraestrutura.

Também contém serviços de segurança, como geração de código, hash de senha e token.

### BarbeariaInfrastructure

Responsável por detalhes técnicos:

- `AppDbContext`;
- configurações do EF Core;
- migration inicial; -> Atualmente apenas com Postgres
- repositories;
- Unit of Work;
- configuração SMTP.

### BarbeariaTests

Contém testes unitários para domínio, value objects, validação e services.

## Fluxo de uma requisição

```text
Cliente HTTP
  ↓
Middlewares globais
  ↓
Autenticação / autorização / antiforgery
  ↓
Controller
  ↓
Application Service
  ↓
Interface de Repository
  ↓
Repository concreto
  ↓
AppDbContext / EF Core
  ↓
PostgreSQL
```

## Dependências de projetos

```text
BarbeariaApi ───────────→ BarbeariaCore
      │
      └───────────────→ BarbeariaInfrastructure ─→ BarbeariaCore

BarbeariaTests ─────────→ BarbeariaCore
      └───────────────→ BarbeariaInfrastructure
```

CORE NÃO VAI DEPENDER DE NINGUÉM

## Composição da aplicação

O `Program.cs` registra os componentes por métodos de extensão:

1. `AddBarbeariaDatabase`;
2. `AddBarbeariaApplication`;
3. `AddBarbeariaInfrastructure`;
4. `AddBarbeariaApiServices`;
5. `AddRateLimiting`;
6. `AddBarbeariaEmail`.

O pipeline executa, nesta ordem:

1. tratamento global de exceções;
2. logging e correlation ID;
3. headers de segurança;
4. Swagger em Development ou HSTS/HTTPS fora dele;
5. routing;
6. CORS;
7. rate limiter;
8. autenticação;
9. autorização;
10. controllers.

## DDD presente

Há evidências concretas de DDD tático:

- entidades com regras de negócio;
- value objects `Cpf`, `Email`, `Phone` e `Senha`;
- exceção de domínio;
- contratos de repositories;
- services de aplicação que orquestram casos de uso.

## Limitações arquiteturais atuais

- Domain e Application estão no mesmo projeto; -> Será mudado futuramente ao implantar FDD
- serviços de token e segurança permanecem no Core;
- organização é majoritariamente por tipo técnico, não por feature;
- não há eventos de domínio, aggregates explicitamente documentados ou testes de arquitetura;
- namespaces misturam `Barbearia` e `BarberShop`;
- há erros de nomenclatura existentes, como `HealtChecks` e `Excepetion`.

Esses pontos são descritos, mas não foram alterados nesta entrega.

# Avaliação Atualizada do Projeto Barbearia

## Comparativo de notas

| Requisito | Nota anterior | Nota atual | Avaliação |
|---|:---:|:---:|---|
| **DDD** | 8,5 | **8,5/10** | Continua forte, mas ainda sem Domain Events, Aggregate Roots explícitos e Specification. |
| **FDD** | 2 | **3/10** | A documentação agora organiza funcionalidades por identificadores como `RF-AUTH-001`, mas ainda não existe um processo FDD formal. |
| **TDD** | 6 | **7,5/10** | Agora existem testes unitários e de integração, incluindo API, autenticação, repositórios e PostgreSQL com Testcontainers. |
| **Análise de requisitos** | 5 | **8,5/10** | Houve uma evolução muito grande: existem requisitos funcionais, pré-condições, regras, critérios de aceitação e requisitos não funcionais. |
| **Performance** | 5 | **6,5/10** | Existem `AsNoTracking`, índices compostos, consultas projetadas e retry do PostgreSQL. Ainda faltam medições e profiling. |
| **Escalabilidade** | 5 | **5,5/10** | Retry e arquitetura ajudam, mas ainda não há cache distribuído, filas, workers ou mensageria. |
| **Segurança** | 7 | **7,8/10** | JWT, cookies, CSRF, validação das configurações, rate limiting, headers e autorização por roles melhoraram a segurança. |
| **APIs REST** | 8 | **8,2/10** | Continua bem estruturada. Ainda faltam versionamento, contratos de erro padronizados e OpenAPI mais detalhado. |
| **Troubleshooting** | 4 | **6/10** | Agora há Correlation ID, logging de requisições, documentação de troubleshooting e uma implementação inicial de Health Check. |

## Resultado consolidado

| Área | Nota atual |
|---|:---:|
| DDD | **8,5/10** |
| FDD | **3/10** |
| TDD | **7,5/10** |
| Análise de requisitos | **8,5/10** |
| Performance | **6,5/10** |
| Escalabilidade | **5,5/10** |
| Segurança | **7,8/10** |
| Integração de APIs REST | **8,2/10** |
| Troubleshooting avançado | **6/10** |

A média simples passou de aproximadamente **5,6 para 6,8**.

## Conclusão

O projeto não está mais no mesmo nível descrito na avaliação anterior.

As maiores evoluções foram:

1. documentação de requisitos e critérios de aceitação;
2. testes de integração com API e PostgreSQL;
3. Correlation ID e logging de requisição;
4. índices de banco planejados;
5. documentação de segurança, performance, arquitetura e troubleshooting.

## Próximas prioridades

1. Ativar efetivamente os Health Checks.
2. Criar uma matriz de rastreabilidade entre requisito, código e teste.
3. Gerar relatório de cobertura de testes.
4. Incluir testes de arquitetura.
5. Padronizar os erros com `ProblemDetails`.
6. Adicionar métricas e tracing com OpenTelemetry.
7. Executar testes de carga e profiling.
8. Implementar auditoria e políticas de autorização mais completas.
