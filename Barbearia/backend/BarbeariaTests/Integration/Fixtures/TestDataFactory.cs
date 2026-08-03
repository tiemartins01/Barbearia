using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.Enum;

namespace BarbeariaTests.Integration;

public static class TestDataFactory
{
    public static Usuario CriarCliente(
        string login = "cliente",
        string email = "cliente@teste.com",
        string cpf = "52998224725",
        string telefone = "11987654321",
        string senha = "123456",
        string nome = "Cliente Teste")
    {
        return new Usuario(
            nome,
            new Email(email),
            new Phone(telefone),
            new Cpf(cpf),
            login,
            Senha.Criar(senha),
            RolePerson.Cliente,
            ativado: true,
            foto: null);
    }

    public static Servicos CriarServico(
        string nome = "Corte",
        int duracao = 30,
        decimal preco = 40m,
        bool ativo = true)
    {
        return new Servicos(nome, duracao, preco, ativo);
    }
}
