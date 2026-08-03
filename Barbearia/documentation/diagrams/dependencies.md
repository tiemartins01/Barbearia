# Dependências entre projetos

```mermaid
flowchart LR
    API[BarbeariaApi] --> CORE[BarbeariaCore]
    API --> INFRA[BarbeariaInfrastructure]
    INFRA --> CORE
    TESTS[BarbeariaTests] --> CORE
    TESTS --> INFRA
```
