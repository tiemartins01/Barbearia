# TESTES AVALIADOS COMO NECESSÁRIOS


## ################### ValidatorsTests ###################

# TESTE 01 - Login válido passa

O que é necessário?

Campos de login e senha preenchidos com valores que estejam cadastrados no banco de dados.

Por que?

É assim que qualquer acesso de login funciona.

Observação:

Os dados são criados como um novo DTOLoginUsuarioValidator onde já passa que a informação de login deve ser não nulo e menor que 50 caracteres. A senha não nula e menor que 128 caracteres.


# TESTE 02 - Login com campos obrigatório vazio deve falhar

O que é necessário?

Campo login ou campo senha esteja vazio, sem valor válido.

Por que?

Faltando qualquer uma dessas informações, não é possível realizar um login no sistema.

Observação:

Informação é criada como um novo DTOLoginUsuarioValidator onde já passa que a informação de login deve ser não nulo e menor que 50 caracteres. A senha não nula e menor que 128 caracteres.

# TESTE 03 - Login com mais de 50 caracteres ou senha com 129 caracteres deve falhar

O que é necessário?

Login com mais de 50 caracteres e senha com mais de 128 caracteres.

Por que?

Sistema foi definido como quantidade máxima de informação em cada um desses campos.

Observação:

Informação é criada como um novo DTOLoginUsuarioValidator onde já passa que a informação de login deve ser não nulo e menor que 50 caracteres. A senha não nula e menor que 128 caracteres.


# verificar se esse teste atende aos requisitos que tem nos value objects

# TESTE 04 - Novo usuário com as informações corretas deve ser cadastrado corretamente.

O que é necessário?

- Campo nome não ser vazio e ter menos de 101 caracteres.
- Campo e-mail não ser vazio e ter menos de 255 caracteres.
- Campo telefone não ser vazio e ter menos de 21 caracteres.
- Campo cpf não ser vazio e ter menos de 15 caracteres.
- Campo login não ser vazio e ter menos de 51 caracteres.
- Campo senha não ser vazio e ter no minimo 6 caracteres e ter menos de 129 caracteres.
- Campo foto ter menos de 501 caracteres e ser diferente de vazio.

Por que?

Sistema foi definido como quantidade máxima de informação em cada um desses campos e essas informações são representadas como básicas para possuir acesso ao sistema.

Observação:

Informação é criada como um novo DTONovoUsuarioValidator com as informações dentro do requisitos criados.


# Login com informações inválidas não deve acessar