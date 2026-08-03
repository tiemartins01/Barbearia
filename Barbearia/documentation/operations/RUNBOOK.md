# Runbook de execução local

## Pré-requisitos

- .NET SDK 8;
- PostgreSQL acessível, localmente ou por Docker;
- credenciais de banco;
- chave JWT com pelo menos 32 bytes;
- SMTP somente quando o envio real estiver habilitado.

## Restaurar e compilar

```bash
cd backend
dotnet restore Barbearia.sln
dotnet build Barbearia.sln
```

## Configuração recomendada

Use User Secrets no projeto da API:

```bash
cd BarbeariaApi
dotnet user-secrets set "ConnectionStrings:PostgreSql" "Host=localhost;Port=5432;Database=barbearia;Username=postgres;Password=SUA_SENHA"
dotnet user-secrets set "Jwt:Key" "SUA_CHAVE_COM_PELO_MENOS_32_BYTES"
dotnet user-secrets set "Jwt:Issuer" "BarbeariaApi"
dotnet user-secrets set "Jwt:Audience" "BarbeariaFrontend"
```

Os nomes devem acompanhar a configuração já usada no código e nos arquivos `appsettings` de exemplo.

## Aplicar migration

```bash
cd backend
dotnet ef database update --project BarbeariaInfrastructure --startup-project BarbeariaApi
```

## Executar API

```bash
cd backend/BarbeariaApi
dotnet run
```

A URL exata é exibida no console. O Swagger fica disponível apenas em Development, em `/swagger`.

## Executar testes

```bash
cd backend
dotnet test Barbearia.sln
```

## Checklist de validação manual

1. abrir Swagger;
2. chamar `GET /csrf`;
3. cadastrar usuário;
4. efetuar login;
5. confirmar cookies `access-token` e `refresh-token`;
6. chamar `GET /login/me`;
7. consultar barbeiros e serviços;
8. consultar horários livres;
9. criar agendamento;
10. consultar próximo agendamento e histórico;
11. efetuar logout.
