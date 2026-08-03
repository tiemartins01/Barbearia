# Fase 6 — Segurança avançada

## Implementado
- rotação de refresh token com família de tokens;
- detecção de reutilização de token já rotacionado;
- revogação da família em caso de reutilização suspeita;
- listagem, revogação individual e revogação total de sessões;
- validação de propriedade ao consultar detalhes de agendamento;
- auditoria enriquecida com User-Agent, método e rota;
- logging específico de respostas 401, 403 e 429;
- CSP e headers adicionais;
- Threat Model documentado.

## Migration necessária
Criar uma migration para as novas colunas de `refresh_token` e `audit_log`.

```powershell
dotnet ef migrations add AddAdvancedSecurity `
  --project .\BarbeariaInfrastructure\BarbeariaInfrastructure.csproj `
  --startup-project .\BarbeariaApi\BarbeariaApi.csproj

dotnet ef database update `
  --project .\BarbeariaInfrastructure\BarbeariaInfrastructure.csproj `
  --startup-project .\BarbeariaApi\BarbeariaApi.csproj
```

## Validações recomendadas
- `dotnet restore`;
- `dotnet build`;
- `dotnet test`;
- login e refresh;
- reutilização de refresh antigo;
- `DELETE /api/v1/auth/sessions/{id}`;
- tentativa de consultar agendamento de outro cliente;
- conferência das novas colunas no `audit_log`.
