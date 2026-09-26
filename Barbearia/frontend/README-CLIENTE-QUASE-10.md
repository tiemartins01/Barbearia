# Frontend Cliente — REST real — versão quase 10/10

Esta pasta contém somente o frontend. O módulo Cliente foi migrado para a arquitetura moderna e não depende mais das páginas `legacy` de Cliente.

## Fluxos preparados

- login real;
- cadastro real;
- recuperação e redefinição de senha;
- perfil;
- serviços;
- barbeiros;
- horários disponíveis;
- criação de agendamento com `Idempotency-Key`;
- próximo agendamento;
- cancelamento preparado pelo contrato REST;
- histórico;
- avaliação.

## Arquitetura

```text
Page
  ↓
Feature Hook (React Query)
  ↓
Feature API
  ↓
shared/api/httpClient
  ↓
ASP.NET Core REST v1
```

Nenhuma página Cliente chama Axios/httpClient diretamente.

## Estado assíncrono

- React Query para server state;
- Zustand para sessão;
- React Hook Form + Zod para formulários;
- loading/error/empty states;
- invalidação de cache depois de mutations;
- cache limpo no logout e quando a sessão expira.

## Segurança frontend

- cookies enviados com `withCredentials`;
- tokens continuam fora de localStorage;
- CSRF centralizado;
- refresh single-flight para evitar várias renovações simultâneas;
- `ProtectedRoute` por role para UX;
- backend continua sendo a autoridade de autorização.

## Único bloqueio funcional que depende do backend

O frontend já possui `cancelAppointment(id)` e o dashboard chama a mutation quando existe `nextAppointment.id`.

Para funcionar de ponta a ponta, o backend precisa:

1. devolver `id` em `DTOProximoAgendamento`;
2. publicar `PATCH /api/v1/appointments/{id}/cancel`.

O frontend não simula esse resultado.

## Execução

Crie `.env.local` a partir de `.env.example` e informe a URL real do backend terminando em `/api/v1`.

```bash
npm ci
npm run lint
npm run build
npm run dev
```
