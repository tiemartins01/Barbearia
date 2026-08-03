-- Ajuste os parâmetros para dados existentes.
EXPLAIN (ANALYZE, BUFFERS, VERBOSE)
SELECT * FROM usuarios WHERE login = 'admin' LIMIT 1;

EXPLAIN (ANALYZE, BUFFERS, VERBOSE)
SELECT * FROM horarios
WHERE cliente_id = 1 AND horario > CURRENT_TIMESTAMP
ORDER BY horario
LIMIT 1;

EXPLAIN (ANALYZE, BUFFERS, VERBOSE)
SELECT * FROM refresh_token
WHERE token = 'TOKEN_DE_TESTE'
LIMIT 1;
