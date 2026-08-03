# Estratégia de testes atual

## Ferramentas

- xUnit 2.9.2;
- Moq 4.20.72;
- Microsoft.NET.Test.Sdk 17.12.0;
- Coverlet Collector 6.0.2.

## Estrutura atual

```text
BarbeariaTests/
├── Unit/
│   ├── Domain/
│   ├── Services/
│   └── Validation/
├── Integration/
│   ├── Controllers/
│   ├── Repositories/
│   ├── Authentication/
│   └── Database/
├── Architecture/
└── Fixtures/
```

Foram identificados 49 métodos declarados com `[Fact]` ou `[Theory]` na avaliação desta versão.

## Cobertura conceitual

### Domain

Testa regras de entidades, horários, serviços, avaliações, usuário e value objects.

### Services

Testa orquestração de login, troca de senha e cadastro/usuário usando mocks.

### Validation

Testa validadores de DTOs com FluentValidation.

## Como executar

Na pasta `backend`:

```bash
dotnet test Barbearia.sln
```

Com coleta de cobertura:

```bash
dotnet test Barbearia.sln --collect:"XPlat Code Coverage"
```

## O que não está coberto nesta versão

- repositories contra PostgreSQL real;
- controllers e pipeline HTTP;
- autenticação baseada em cookies;
- refresh token de ponta a ponta;
- CSRF;
- rate limiting;
- migrations;
- concorrência de agendamento;
- health checks;
- regras de arquitetura.

## Relação com TDD

Os testes demonstram preocupação com testabilidade e regras. Entretanto, o estado final do código não comprova sozinho que o ciclo Red → Green → Refactor foi seguido. Para demonstrar TDD, mantenha commits pequenos e registre a evolução de cada caso de uso.

## Próxima prioridade de testes

1. criar `BarbeariaIntegrationTests`;
2. subir PostgreSQL isolado para os testes;
3. testar login, autorização e CSRF via `WebApplicationFactory`;
4. testar persistência e rollback;
5. testar corrida de duas reservas para o mesmo horário;
6. adicionar `BarbeariaArchitectureTests`.
