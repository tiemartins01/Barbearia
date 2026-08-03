# Banco de dados

## Provider atual

O backend usa exclusivamente **PostgreSQL** nesta versão, através de:

- Entity Framework Core 8.0.11;
- Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11;
- connection string `ConnectionStrings:PostgreSql`.

Futuramente a ideia é MySQL e SQLServer também

A configuração habilita retry para falhas transitórias:

- máximo de 5 tentativas;
- intervalo máximo de 10 segundos.

## Contexto

O `AppDbContext` representa as tabelas relacionadas às entidades:

- `Usuario`;
- `Barbeiro`;
- `Servicos`;
- `Horarios`; -> Em avaliação, entretanto, provavelmente o nome vai ser alterado para agendamento.
- `Avaliacoes`;
- `RefreshToken`.

As configurações de mapeamento ficam em `BarbeariaInfrastructure/Data/Configuration`.

## Migration existente

Há uma migration inicial:

```text
20260727222643_InitialCreate
```

O snapshot correspondente também está presente. -> Realiza a comparação dos bancos

## Responsabilidades dos repositories

- `LoginRepository`: consulta usada na autenticação;
- `NovoClienteRepository`: persistência de novo cliente;
- `AbaClienteRepository`: dados pessoais, histórico, barbeiros e avaliações;
- `ProximoAtendimentoRepository`: agenda e disponibilidade;
- `ServicosAtivosRepository`: consulta de serviços;
- `EmailEsqueciSenhaRepository`: recuperação de senha;
- `TrocaSenhaRepository`: troca de senha;
- `RefreshRepository`: persistência e revogação de refresh tokens;
- `UnitOfWorksRepository`: confirmação transacional pelo contexto.

## Comandos de migrations

A partir da pasta `backend`:

```bash
dotnet ef migrations add NomeDaMigration \
  --project BarbeariaInfrastructure \
  --startup-project BarbeariaApi

dotnet ef database update \
  --project BarbeariaInfrastructure \
  --startup-project BarbeariaApi
```

## Limitações atuais

- não existem providers ou migrations independentes para SQL Server e MySQL;
- não há testes de integração do EF Core no projeto entregue;
- a proteção contra duas reservas concorrentes do mesmo horário precisa ser validada no banco;
- health check de banco existe como classe, mas não está registrado e mapeado no pipeline atual.
