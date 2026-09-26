# Frontend Cliente — REST real

Esta pasta foi gerada a partir do frontend do ZIP `Barbearia-feature-ddd-business-rules(1).zip` e altera somente o frontend.

## O que já foi migrado
- login real + `/auth/me` + cookies;
- refresh real com single-flight;
- CSRF nas mutations;
- cadastro;
- recuperação e redefinição de senha;
- perfil;
- serviços;
- barbeiros;
- horários disponíveis;
- criação de agendamento com `Idempotency-Key`;
- próximo agendamento;
- histórico paginado;
- avaliação;
- loading/error/empty states;
- rotas Cliente protegidas por role no frontend;
- contratos TypeScript baseados nos DTOs atuais do backend.

## Limitações que dependem do backend atual
1. O backend ainda não publica `PATCH /api/v1/appointments/{id}/cancel`.
2. `DTOProximoAgendamento` não possui `Id`; portanto o frontend não consegue identificar com segurança o próximo agendamento para cancelamento.
3. O endpoint REST `GET /api/v1/appointments/{id}` está na mesma action que um POST legado com body; recomenda-se separar a action v1.
4. O frontend consegue tratar o formato atual de erro `{ sucesso, codigo, mensagem, traceId }`, mas os Controllers ainda possuem algumas respostas manuais fora desse formato.
5. Os use cases/repositories ainda precisam completar a propagação de `CancellationToken` conforme o roadmap.

## Configuração
Copie `.env.example` para `.env.local` e aponte para a base v1, por exemplo:

```env
VITE_API_URL=http://localhost:5244/api/v1
```

## Execução
```bash
npm ci
npm run build
npm run dev
```

Os testes automatizados não foram adicionados nesta entrega, conforme solicitado.
