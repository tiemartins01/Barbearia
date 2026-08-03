# Mudança realizada nesssa ultima versão.

- No ServiceRefreshToken foi retirado a linha de código do try, do begin transaction e do commit, porque com apenas 1 SaveChanges, não foi visto como necessário fazer o uso do mesmo.
- Inserção dos códigos faltantes nos itens de DomainException
