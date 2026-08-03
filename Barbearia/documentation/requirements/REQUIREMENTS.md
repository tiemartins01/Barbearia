# Requisitos funcionais

## RF-AUTH-001 — Autenticar usuário

### Objetivo

Permitir que um usuário ativo e não bloqueado acesse o sistema.

### Entrada

- Login.
- Senha.

### Pré-condições

- O usuário precisa existir.
- O usuário precisa estar ativo.
- O usuário não pode estar bloqueado.

### Regras de negócio

- RN-AUTH-001: o login deve ser normalizado.
- RN-AUTH-002: usuário inexistente não deve ser revelado como inexistente.
- RN-AUTH-003: senha incorreta incrementa tentativas de falha de login.
- RN-AUTH-004: cinco falhas bloqueiam temporariamente o usuário.
- RN-AUTH-005: login válido zera as tentativas de falha de login.
- RN-AUTH-006: refresh token deve ser armazenado de forma segura(cookies).

### Critérios de aceitação

- Dado um usuário válido, quando informar a senha correta, então o login deve ser realizado.
- Dado um usuário inexistente, quando tentar login, então deve receber resposta genérica.
- Dado um usuário bloqueado, quando tentar login, então deve receber resposta genérica.
- Dada uma senha incorreta, a quantidade de tentativas deve ser incrementada.

### Requisitos não funcionais

- RNF-001: não registrar senha ou token.
- RNF-002: consulta de login deve usar índice.
- RNF-003: falhas devem possuir correlation ID.
- RNF-004: operações devem aceitar cancelamento.

## RF-AUTH-002 — Consultar usuário autenticado

### Objetivo

Verificar a autenticação do usuário.

### Entrada

- Access-token presente no cookie.

### Pré-condições

- Ter refresh válido
- Refresh não está revogado.
- Tempo de expirado menor que horário atual.
- O usuário precisa existir.
- O usuário precisa estar ativo.
- O usuário não pode estar bloqueado.

### Regras de negócio

- RN-AUTH-001: Deve ter valor de refresh
- RN-AUTH-002: Resfresh não pode estar revogado
- RN-AUTH-003: Tempo de refresh ainda válido
- RN-AUTH-004: Usuário inexistente não deve ser revelado como inexistente.
- RN-AUTH-005: Usuário precisa estar ativo.
- RN-AUTH-006: Usuário não pode estar bloqueado.

### Critérios de aceitação

- Realizar a leitura do token guardado no cookie.
- Não ter expirado o tempo do token.

### Requisitos não funcionais

- RNF-001: Verifica token no navegador.
- RNF-002: Confere no banco de dados se ele pode ser reutilizado.
- RNF-003: Falhas devem possuir correlation ID.
- RNF-004: Operações devem aceitar cancelamento.
- RNF-005: Atualiza a informação nova no Banco de dados.

## RF-AUTH-003 — Atualizar access token

### Objetivo

Atualizar o token.

### Entrada

 Login.
- Senha.

### Pré-condições

- O usuário precisa existir.
- O usuário precisa estar ativo.
- O usuário não pode estar bloqueado.

### Regras de negócio

- RN-AUTH-001: o login deve ser normalizado.
- RN-AUTH-002: usuário inexistente não deve ser revelado como inexistente.
- RN-AUTH-003: senha incorreta incrementa tentativas de falha de login.
- RN-AUTH-004: cinco falhas bloqueiam temporariamente o usuário.
- RN-AUTH-005: login válido zera as tentativas de falha de login.
- RN-AUTH-006: refresh token deve ser armazenado de forma segura(cookies).

### Critérios de aceitação

- Dado um usuário válido, quando informar a senha correta, então o login deve ser realizado.
- Dado um usuário inexistente, quando tentar login, então deve receber resposta genérica.
- Dado um usuário bloqueado, quando tentar login, então deve receber resposta genérica.
- Dada uma senha incorreta, a quantidade de tentativas deve ser incrementada.

### Requisitos não funcionais

- RNF-001: não registrar senha ou token.
- RNF-002: consulta de login deve usar índice.
- RNF-003: falhas devem possuir correlation ID.
- RNF-004: operações devem aceitar cancelamento.

## RF-AUTH-004 — Encerrar sessão

### Objetivo

Encerrar sessão do usuário.

### Entrada

- Estar logado.

### Pré-condições

- O usuário precisa estar logado.
- Token no cookie.
- Token não pode estar revogado

### Regras de negócio

- RN-AUTH-001: Usuário deve estar logado no sistema.
- RN-AUTH-002: Token diferente de nulo para poder continuar.
- RN-AUTH-003: Token não pode estar revogado.

### Critérios de aceitação

-  Usuário válido com token não expirado.

### Requisitos não funcionais

- RNF-001: Verificar o token.
- RNF-002: Revogar token antigo.
- RNF-003: Criar novo refresh token.
- RNF-004: operações devem aceitar cancelamento.

## RF-AUTH-005 — Bloquear usuário por tentativas inválidas

### Objetivo

Bloquear usuário que aparentemente não tem acesso

### Entrada

- Login
- Senha

### Pré-condições

- Não ter conhecimento da senha.

### Regras de negócio

- RN-AUTH-001: Usuário deve errar 5 vezes a senha do login.

### Critérios de aceitação

-  Usuário com senha correta.

### Requisitos não funcionais

- RNF-001: Incrementar quantidade cada vez que erra.
- RNF-002: Adiciona 5 minutos.

## RF-AUTH-006 — Desbloquear usuário após período

### Objetivo

Usuário desbloqueado após tempo

### Entrada


### Pré-condições

- Estar bloqueado.

### Regras de negócio

- RN-AUTH-001: Mais de 5 minutos depois de ter bloqueado.
- RN-AUTH-002: Bloqueio difernte de nulo.

### Critérios de aceitação

-  Usuário ter esperado o tempo pós bloqueio.

### Requisitos não funcionais

- RNF-001: Verificar se ainda está bloqueado.

## RF-AUTH-007 — Revogar refresh token

### Objetivo

Revogar refresh que não foi atualizado ainda.

### Entrada


### Pré-condições

- Mais de 7 dias logado sem realiza o refresh.
- Suspeita de token roubado.

### Regras de negócio

- RN-AUTH-001: Token diferente de nulo para poder continuar.
- RN-AUTH-002: Token não pode estar revogado.

### Critérios de aceitação

-  Solicitação manual do token.

### Requisitos não funcionais

- RNF-001: Verificar se o antigo está revogado.

## RF-USER-001 — Cadastrar cliente

### Objetivo

Cadastrar um novo cliente

### Entrada

- Nome
- E-mail
- Telefone
- CPF
- Login
- Senha
- Foto

### Pré-condições

- Todas as informações preenchidas
- Informações tem que respeitar as configurações de ValueObjects.
- Não é aceito valores vazio ou preenchidos com " ".

### Regras de negócio

- RN-AUTH-001: Nome e login preencido.
- RN-AUTH-002: Verifica duplicidade dos itens.

### Critérios de aceitação

- Não apresentar valores duplicados.
- Campos preenchidos.

### Requisitos não funcionais

- RNF-001: Verificar no banco cada valor se é duplicado ou não.
- RNF-002: Normaliza campos.

## RF-USER-002 — Consultar dados pessoais

### Objetivo

Consultar dados pessoais

### Entrada

- Id presente no token

### Pré-condições

- Usuário estar logado.
- Pesquisar ele apenas.

### Regras de negócio


### Critérios de aceitação

- Buscar apenas os dados pessoais.

### Requisitos não funcionais

- RNF-001: Verifica se os dados foram buscados no banco.

## RF-USER-003 — Alterar dados pessoais

### Objetivo

Alterar os dados pessoais

### Entrada

- O item que você quer alterar.

### Pré-condições

- Estar conforme as condições dos ValueObjects.

### Regras de negócio

- RN-AUTH-001: Usuário não nulo.
- RN-AUTH-002: Em caso de alteração de senha, não pode ser vazia
- RN-AUTH-003: Senha tem que ser conferida com a senha antiga.

### Critérios de aceitação

- Alterar apenas os próprios dados pessoais.

### Requisitos não funcionais

- RNF-001: Conferência da senha com a antiga.

## RF-USER-004 — Recuperação de senha

### Objetivo

Recuperar senha do login

### Entrada

- Senha antiga
- Senha nova

### Pré-condições

- Senha antiga correta.

### Regras de negócio

- RN-AUTH-001: Senha diferente de nulo.
- RN-AUTH-002: Verifica senha antiga
- RN-AUTH-003: Altera a senha.

### Critérios de aceitação

- Senha antiga correta.

### Requisitos não funcionais

- RNF-001: Conferência da senha com a antiga no banco.

## RF-PASSWORD-001 — Solicitar recuperação de senha

### Objetivo

Solicitar a recuperação de senha

### Entrada

- E-mail

### Pré-condições

- E-mail válido e cadastrado.

### Regras de negócio

- RN-AUTH-001: Verifica e-mail inserido.

### Critérios de aceitação

- E-mail registrado no banco de dados.

### Requisitos não funcionais

- RNF-001: Normalização do e-mail.
- RNF-002: Código de recuperação gerado.

## RF-PASSWORD-002 — Validar código de recuperação

### Objetivo

Validar o código para recuperar senha 

### Entrada

- E-mail enviado

### Pré-condições

- E-mail enviado

### Regras de negócio

- RN-AUTH-001: Verifica e-mail inserido e adiciona o código + tempo de expiração no registro desse usuário.

### Critérios de aceitação

- Código de 6 digitos.

### Requisitos não funcionais

- RNF-001: Insere o tempo + código no registro do usuário

## RF-PASSWORD-003 — Trocar senha

### Objetivo

Realizar a troca da senha

### Entrada

- E-mail
- Código
- Senha
- SenhaRepetida

### Pré-condições

- Código adicionado no registro do usuário
- Senha com pelo menos 6 digitos.

### Regras de negócio

- RN-AUTH-001: Usuário exisetente.
- RN-AUTH-002: Usuário Ativo.
- RN-AUTH-003: Usuário não bloqueado.
- RN-AUTH-004: Senhas iguais.

### Critérios de aceitação

- Senhas iguais.
- Código correto.

### Requisitos não funcionais

- RNF-001: Verificação no banco das informações

## RF-PASSWORD-004 — Invalidar código após utilização

### Objetivo

Apagar informações sobre código após troca

### Entrada

### Pré-condições

- Ter realizado a troca da senha.

### Regras de negócio

- RN-AUTH-001: Senhas iguais

### Critérios de aceitação

- Ter trocado a senha com todas as informações corretas.

### Requisitos não funcionais

- RNF-001: Apagar o código do banco
- RNF-002: Apagar o tempo de espiração

# RF-SERVICE-001 — Consultar serviços ativos

### Objetivo

Verificar serviços ativos

### Entrada

### Pré-condições

- Ter serviços ativos

### Regras de negócio

- RN-AUTH-001: Serviço precisa estar ativo

### Critérios de aceitação

- Campo de ativo = true

### Requisitos não funcionais

- RNF-001: Verificar no banco se está ativo

# RF-APPOINTMENT-001 — Consultar próximo agendamento

### Objetivo

Verificar próximo agendamento

### Entrada

- ID do usuário pego no token

### Pré-condições

- Ter um agendamento pós a data + hora atual

### Regras de negócio

- RN-AUTH-001: Ter um agendamento futuro

### Critérios de aceitação

- Agendamento futuro

### Requisitos não funcionais

- RNF-001: Verificar no banco se existe um agendamento futuro

# RF-APPOINTMENT-002 — Consultar histórico de atendimentos

### Objetivo

Consultar histórico do cliente

### Entrada

- ID do usuário pego no token

### Pré-condições

- Ter pelo menos um serviço feito realizado 

### Regras de negócio

- RN-AUTH-001: Ter um agendamento concluido
- RN-AUTH-002: Possibilidade de avaliar um atendimento
- RN-AUTH-002: Escolher quantos serviços quer ver

### Critérios de aceitação

- Ter serviços realizado

### Requisitos não funcionais

- RNF-001: Verificar no banco todos os serviços recebidos

# RF-APPOINTMENT-003 — Avaliar atendimento

### Objetivo

Avaliar um agendamento

### Entrada

- ID do usuário pego no token

### Pré-condições

- Ter um agendamento como concluido

### Regras de negócio

- RN-AUTH-001: O agendamento deve ter sido concluido
- RN-AUTH-002: Inserir nota e comentario sendo opcional.

### Critérios de aceitação

- Notas de 1 a 5.
- Até 128 caracteres. 

### Requisitos não funcionais

- RNF-001: Verificar se o comentário tem mais de 128 caracteres
- RNF-001: Verificar se o comentário está entre 1 e 5.
