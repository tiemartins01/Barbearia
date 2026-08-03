# Threat Model — Barbearia API

## Escopo
Autenticação, sessões, dados pessoais, agendamentos, auditoria e infraestrutura de observabilidade.

## Ativos protegidos
- credenciais e tokens;
- CPF, e-mail e telefone;
- sessões autenticadas;
- agendamentos e avaliações;
- trilhas de auditoria;
- segredos de infraestrutura.

## Ameaças e controles

| Ameaça | Controle |
|---|---|
| Roubo de access token | Cookie HttpOnly, Secure em HTTPS, expiração curta |
| Roubo/reutilização de refresh token | rotação, família de tokens e revogação da família |
| CSRF | token antiforgery nas operações mutáveis |
| Brute force | bloqueio de usuário e rate limiting |
| IDOR/acesso a recurso alheio | validação do proprietário do agendamento |
| Vazamento por logs | campos sensíveis mascarados e logging sem corpo/token |
| Replay de criação | Idempotency-Key |
| Vazamento de detalhes internos | ProblemDetails e middleware centralizado |
| Alteração sem rastreabilidade | audit_log com usuário, IP, User-Agent, rota e correlação |
| Clickjacking/XSS em páginas auxiliares | CSP, frame-ancestors e X-Frame-Options |

## Riscos residuais
- segredos precisam estar em secret manager em produção;
- rate limiting em memória não é global em múltiplas instâncias;
- auditoria precisa de retenção e acesso restrito;
- dependências devem ser verificadas continuamente por scanner.
