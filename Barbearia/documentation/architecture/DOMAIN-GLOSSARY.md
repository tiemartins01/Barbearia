# Glossário do domínio

## Usuário

Pessoa que acessa o sistema. Armazena identidade, credenciais, role, estado de ativação, tentativas de login e dados relacionados à recuperação de senha.

## Cliente

Papel de usuário autorizado a acessar as rotas de cliente, consultar serviços e barbeiros, agendar horários, consultar histórico e avaliar atendimento.

## Barbeiro

Profissional relacionado a um usuário e usado na composição dos horários e atendimentos.

## ADMIN

Gerir o sistema com acesso total

## Serviço

Procedimento oferecido pela barbearia. Pode estar ativo ou inativo e participa do agendamento.

## Horário / Agendamento

Representa um atendimento associado a cliente, barbeiro, serviço, data/hora e status.

## Avaliação

Feedback relacionado a um atendimento concluído, com regras próprias de domínio.

## Refresh token

Credencial persistida para renovar o access token sem exigir novo login. Pode ser revogada.

## Value objects

- `Email`: representa e valida um endereço de e-mail;
- `Cpf`: representa e valida CPF;
- `Phone`: representa e valida telefone;
- `Senha`: representa regras relacionadas à senha.

## Status de agendamento

Definido pelo enum `StatusAgendamento`. Deve ser usado para expressar o ciclo de vida do atendimento sem strings soltas.

## Role

Definida por `RolePerson` e convertida em claim de autorização. A API atual protege a área do cliente com a role textual `Cliente`.
