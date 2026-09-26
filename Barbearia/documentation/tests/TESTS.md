# Estratégia de testes atual

## Ferramentas

- xUnit 2.9.2;
- Moq 4.20.72;
- Microsoft.NET.Test.Sdk 17.12.0;
- Coverlet Collector 6.0.2.

## Estrutura atual


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


## Relação com TDD

Os testes demonstram preocupação com testabilidade e regras. Entretanto, o estado final do código não comprova sozinho que o ciclo Red → Green → Refactor foi seguido. Para demonstrar TDD, mantenha commits pequenos e registre a evolução de cada caso de uso. Isso se da principalmente pelo crescimento não planejado do sistema, fazendo com que após bastante evolução, não tenha sido feito pela forma correta. Com isso, o sistema está sendo estabilizado para que seja feito da forma correta e atenda ao requisito.

## Próxima prioridade de testes