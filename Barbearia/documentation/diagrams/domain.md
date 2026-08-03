# Visão simplificada do domínio

```mermaid
erDiagram
    USUARIO ||--o| BARBEIRO : "pode representar"
    USUARIO ||--o{ HORARIO : "agenda como cliente"
    BARBEIRO ||--o{ HORARIO : "atende"
    SERVICO ||--o{ HORARIO : "é escolhido em"
    HORARIO ||--o| AVALIACAO : "pode receber"
    USUARIO ||--o{ REFRESH_TOKEN : "possui"
```

O diagrama é conceitual e serve para navegação. Os detalhes definitivos de cardinalidade, chaves e nomes devem ser conferidos nas configurações EF Core e migrations.
