# Performance e escalabilidade

## Práticas existentes

- operações assíncronas em services e repositories;
- consultas de leitura com `AsNoTracking` em partes da infraestrutura;
- projeção para DTOs em consultas;
- paginação no histórico (`page` e `pageSize`);
- rate limiting para proteger recursos sensíveis tanto em login como em ip;
- retry do PostgreSQL para falhas transitórias;
- lifetime `Scoped` para DbContext, services e repositories;
- medição de duração de cada requisição no middleware de logging. -> RequestLoggingMiddleware.

## Pontos que exigem atenção

### Paginação

O histórico já recebe paginação. Outras coleções que crescerem devem seguir o mesmo princípio. `pageSize` deve ter limite máximo para impedir consultas excessivas.

### Concorrência de agenda

A disponibilidade consultada antes da gravação não é suficiente contra duas requisições simultâneas. A garantia definitiva deve existir no banco, por meio de restrição ou índice único adequado e tratamento da violação. Está em processo pois utilizar filter pode não ser aceita pelos 3 bancos.

### Observabilidade

O tempo de resposta é registrado em logs, mas não há métricas agregadas, percentis, dashboards ou tracing distribuído.

## Próximas medições recomendadas

- p50, p95 e p99 de latência;
- taxa de erro por endpoint;
- número de consultas e tempo de banco;
- consumo de conexões;
- throughput de login e agendamento;
- conflitos de reserva;
- volume e tempo de envio de e-mails.
