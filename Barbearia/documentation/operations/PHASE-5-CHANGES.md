# Fase 5 — Qualidade, segurança e observabilidade

## Implementado

- OpenTelemetry para ASP.NET Core, HttpClient, runtime e métricas próprias.
- Endpoint Prometheus em `/metrics`.
- Exportação OTLP para o OpenTelemetry Collector e Tempo.
- Prometheus, Grafana, Tempo e Collector no Docker Compose.
- Auditoria automática de inclusão, alteração e exclusão, com mascaramento de campos sensíveis.
- Policies `ActiveUser`, `ClientOnly`, `BarberOnly` e `AdminOnly`.
- Listagem e revogação de todas as sessões do usuário.
- Fluxo TDD documentado.
- Scripts de `EXPLAIN (ANALYZE, BUFFERS)`.

## Migration necessária

A tabela `audit_log` foi adicionada. Gere a migration e aplique no banco.

```powershell
dotnet ef migrations add AddAuditLog --project BarbeariaInfrastructure --startup-project BarbeariaApi
dotnet ef database update --project BarbeariaInfrastructure --startup-project BarbeariaApi
```

## Execução

```powershell
docker compose up -d
dotnet restore
dotnet build
dotnet test
dotnet run --project backend/BarbeariaApi
```

Prometheus: `http://localhost:9090`  
Grafana: `http://localhost:3000` (`admin` / `admin`)  
Métricas da API: `http://localhost:5077/metrics`
