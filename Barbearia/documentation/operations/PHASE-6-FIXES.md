# Correções da Fase 6

## Problemas corrigidos

- Datas de refresh token configuradas como `timestamp with time zone`.
- Conversão defensiva de datas recebidas pelo repositório para UTC.
- Migration `AddScalabilityFoundation` registrada corretamente para o EF Core.
- Migration `AddAdvancedSecurity` incluída no projeto.
- Tabela `audit_log` criada uma única vez pela migration de segurança.
- Testes de senha atualizados para as mensagens padronizadas.
- Login inválido atualizado para esperar `401 Unauthorized`.

## Validação local

Em banco de desenvolvimento descartável:

```powershell
docker compose down -v
docker compose up -d postgres

dotnet ef database update `
  --project .\BarbeariaInfrastructure\BarbeariaInfrastructure.csproj `
  --startup-project .\BarbeariaApi\BarbeariaApi.csproj

dotnet build
dotnet test
```

A conexão executada fora do container deve usar a porta publicada no host, normalmente `5433`.
