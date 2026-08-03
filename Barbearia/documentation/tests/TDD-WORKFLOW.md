# Fluxo TDD

Para toda nova regra: **Red → Green → Refactor**.

1. Red: escrever um teste que expresse o comportamento e falhe pelo motivo esperado.
2. Green: implementar o mínimo necessário.
3. Refactor: remover duplicação e melhorar o design sem alterar o comportamento.

## Evidência em commits

- `test: adiciona cenário ... [FEAT-...]`
- `feat: implementa comportamento ... [FEAT-...]`
- `refactor: melhora ... [FEAT-...]`

## Cobertura

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

TDD não é comprovado apenas pela existência de testes; o histórico das próximas features deve preservar o ciclo.
