# Fase 2 — Evolução do DDD

## Alterações realizadas

### 1. Aggregate Root

Foi criada a abstração `AggregateRoot`, responsável por manter eventos produzidos durante mudanças relevantes no domínio.

Arquivos:

- `Domain/Common/AggregateRoot.cs`;
- `Domain/Common/IDomainEvent.cs`.

### 2. Usuario como Aggregate Root

`Usuario` agora concentra explicitamente as regras de:

- normalização de login;
- alteração de dados;
- bloqueio por falhas;
- ativação;
- recuperação de senha;
- alteração de senha.

Foram adicionadas sobrecargas que recebem a data atual, permitindo comportamento determinístico sem remover os métodos já utilizados pela aplicação.

### 3. Horarios como Aggregate Root

`Horarios` passou a proteger:

- criação somente para datas futuras;
- estado inicial obrigatório `Agendado`;
- transições válidas de status;
- geração de evento a cada alteração de estado.

A alteração de status foi centralizada em um único método privado, removendo repetição.

### 4. Domain Events

Eventos adicionados para usuário e agendamento. Eles representam fatos que já ocorreram no domínio e podem ser usados futuramente por auditoria, notificações, mensageria e observabilidade.

### 5. Compatibilidade

Os métodos públicos usados pelos services foram mantidos. Não houve alteração de tabela nem migration nesta fase.

## O que ainda não foi feito

- dispatcher de Domain Events após o commit;
- handlers de e-mail, auditoria ou mensageria;
- restrição única de concorrência para horários;
- Specification Pattern;
- separação em Bounded Contexts físicos.

Esses pontos devem ser implementados em etapas posteriores para não misturar modelagem de domínio com infraestrutura.
