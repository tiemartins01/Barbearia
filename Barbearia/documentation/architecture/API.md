# API atual

## Convenções gerais

- A API usa controllers ASP.NET Core.
- Respostas de erros de domínio são padronizadas pelo `ErrorHandlingMiddleware`. -> Transforma em resposta HTTP padronizada
- Endpoints mutáveis são protegidos globalmente por antiforgery, exceto onde há `[IgnoreAntiforgeryToken]`.  -> protege API contra ataques CSRF quando a autenticação usa cookies. Sem antiforgery, um site malicioso poderia tentar fazer o navegador do usuário enviar uma operação para sua API usando os cookies já existentes.
- O token JWT é lido do cookie `access-token`.
- Endpoints com papel de cliente exigem role `Cliente`. 

## Autenticação

### `POST /login`

Autentica um usuário.

- anonimato: permitido na prática;
- antiforgery: ignorado explicitamente;
- limite: 5 requisições por minuto; -> Mais que isso da erro 427.
- body: `DTOLoginUsuario` (`Nome`, `Senha`);
- sucesso: `204 No Content`;
- efeito: grava cookies `access-token` e `refresh-token`.

### `GET /login/me`

Retorna o usuário autenticado. -> Verifica se existe e se é válido o Cookie

- autorização: usuário autenticado;
- sucesso: `200 OK` com `Id`, `Nome` e `Role`.

### `POST /login/refresh`

Renova os tokens usando o cookie `refresh-token`.

- limite: 20 requisições por minuto;
- sucesso: `204 No Content`;
- sem refresh token: `401 Unauthorized`.

### `POST /login/logout`

Revoga o refresh token quando possível e remove os cookies.

- atributo atual: `[AllowAnonymous]`;
- sucesso: `204 No Content`.

## CSRF

### `GET /csrf`

Gera e armazena o token antiforgery.

- anonimato: permitido;
- sucesso: `200 OK` com `{ "token": "..." }`;
- header esperado nas requisições protegidas: `X-CSRF-TOKEN`;
- cookie antiforgery: `XSRF-TOKEN`.

## Cadastro e senha

### `POST /cadastro`

Cadastra um novo cliente.

- body: `DTONovoUsuario`;
- limite: 5 requisições a cada 5 minutos;
- sucesso atual: `201 Created`.

### `POST /envioe`

Solicita envio de e-mail para recuperação de senha.

- body: `DTOEnviarEmail`;
- sucesso: `200 OK`.

Observação: existe uma policy chamada `recuperacao-senha`, mas o controller atual não aplica `[EnableRateLimiting("recuperacao-senha")]`.

### `POST /trocar`

Troca a senha usando código de recuperação.

- body: `DTOMudarSenha`;
- limite: 10 requisições a cada 10 minutos;
- sucesso: `200 OK`.

## Área do cliente

Todas as rotas abaixo exigem role `Cliente`.

### `GET /cliente/barbeiros`

Lista barbeiros cadastrados. (Verificar porque aqui tem que estar ativo também, se não o barbeiro não pode atender)

### `GET /cliente/historico?page=1&pageSize=10`

Retorna histórico paginado do cliente autenticado.

### `GET /cliente/dados`

Retorna os dados pessoais do cliente autenticado.

### `POST /cliente/infoHorario`

Retorna detalhes de um horário informado no body `DTOInfoHorario`.

### `POST /cliente/alterarDados`

Altera dados pessoais usando `DTOAlterandoDados`. O ID é substituído pelo ID do usuário autenticado.

### `POST /cliente/avaliacao`

Registra avaliação usando `DTOAvaliacao` e o usuário autenticado. -> Apenas horário concluidos

## Agendamento

Todas as rotas exigem role `Cliente`.

### `GET /agendamento/proximo`

Retorna o próximo atendimento do cliente autenticado.

### `GET /agendamento/horarioslivres?id_barbeiro={id}&data={yyyy-MM-dd}`

Consulta horários disponíveis de um barbeiro em uma data.

### `POST /agendamento/marcar`

Agenda um horário com body `DTOMarcarHorario`.

## Serviços

### `GET /servicos/ativos`

Lista serviços ativos.

- autorização: role `Cliente`.

## Modelo global de erro de domínio

```json
{
  "sucesso": false,
  "codigo": "CODIGO_DO_ERRO",
  "mensagem": "Descrição segura do erro.",
  "traceId": "identificador-da-requisicao"
}
```

Erros inesperados retornam status 500, código `INTERNAL_ERROR` e mensagem genérica.