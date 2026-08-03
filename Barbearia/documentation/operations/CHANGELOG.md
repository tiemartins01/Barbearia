# Changelog da documentação

## Backend Foundation v1.0 — 2026-08-01

### Adicionado

- Testes em BarbeariaTests/Services/LoginServiceTests.cs

Adicionar ou melhorar testes para:

login e senha vazios;
normalização do login;
usuário inexistente;
usuário desativado;
usuário bloqueado;
senha inválida;
incremento das tentativas;
persistência da falha;
reset das tentativas após sucesso;
geração dos tokens;
armazenamento do refresh token;
falha ao salvar no banco;
cancelamento da operação;
ausência de persistência quando a validação inicial falha.

### Código da aplicação

Inicio da documentação, antes desse item foi criado conforme aprendizado, contudo, agora está sendo conforme cronograma.

Nenhum arquivo de código, configuração existente, migration ou teste foi alterado por esta entrega. Foi adicionada somente a pasta `documentation/`. 


Inserção de vários testes para cobrir:

BarbeariaTests/Services/LoginServiceTests.cs

login e senha vazios;
normalização do login;
usuário inexistente;
usuário desativado;
usuário bloqueado;
senha inválida;
incremento das tentativas;
persistência da falha;
reset das tentativas após sucesso;
geração dos tokens;
armazenamento do refresh token;
falha ao salvar no banco;
cancelamento da operação;
ausência de persistência quando a validação inicial falha.

## v0.4 — Fase 4

- idempotência persistida para criação de agendamentos;
- proteção contra concorrência com índice parcial único;
- Outbox Pattern integrado ao `SaveChangesAsync`;
- worker em background para mensagens do Outbox;
- documentação FDD e matriz de rastreabilidade consolidadas;
- ADR de idempotência e Outbox.
