# Modelo de Domínio — Fase 2

## Objetivo

Registrar os limites dos agregados e as invariantes protegidas pelo domínio.

## Aggregate Root: Usuario

Responsável por:

- identidade e dados pessoais;
- ativação e desativação;
- tentativas e bloqueio de login;
- geração e invalidação de código de recuperação;
- alteração de senha.

### Invariantes

- nome e login são obrigatórios;
- login é armazenado normalizado;
- e-mail, telefone, CPF e senha são Value Objects obrigatórios;
- após cinco falhas de login, o usuário fica temporariamente bloqueado;
- código de recuperação expira e possui limite de tentativas;
- apenas um código ativo pode ser usado para alteração de senha.

## Aggregate Root: Horarios

Representa o ciclo de vida do agendamento.

### Invariantes

- cliente, barbeiro e serviço precisam possuir identificadores válidos;
- o horário precisa estar no futuro;
- um novo agendamento nasce no estado `Agendado`;
- somente `Agendado` pode virar `Concluido` ou `Cancelado`;
- somente `Concluido` pode virar `Avaliado`;
- o status não pode ser alterado diretamente fora do Aggregate Root.

## Entidades auxiliares

- `Barbeiro`: associação entre usuário e atividade profissional;
- `Servicos`: catálogo de serviços oferecidos;
- `Avaliacoes`: registro da nota e comentário associados a um atendimento;
- `RefreshToken`: persistência da sessão renovável.

## Value Objects

- `Email`;
- `Cpf`;
- `Phone`;
- `Senha`.

Os Value Objects encapsulam validação, normalização e comportamento, evitando o uso de strings sem significado de domínio.

## Domain Events adicionados

### Usuário

- `UsuarioCriadoDomainEvent`;
- `UsuarioBloqueadoDomainEvent`;
- `SenhaAlteradaDomainEvent`;
- `RecuperacaoSenhaSolicitadaDomainEvent`;
- `UsuarioAtivacaoAlteradaDomainEvent`.

### Agendamento

- `AgendamentoCriadoDomainEvent`;
- `AgendamentoStatusAlteradoDomainEvent`.

Nesta fase, os eventos são registrados pelos agregados. O despacho para handlers externos deve ocorrer após a confirmação da unidade de trabalho, evitando efeitos colaterais antes do commit.
