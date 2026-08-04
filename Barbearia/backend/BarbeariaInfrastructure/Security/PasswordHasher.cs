using BarbeariaCore.Application.Interfaces;

namespace BarbeariaInfrastructure.Security
{
    public class PasswordHasher : IPasswordHash
    {
        // GERA O HASH DA SENHA
        public string Hash(string senha)
            => BCrypt.Net.BCrypt.HashPassword(senha);
        // VERIFICA SE A SENHA NO BANCO É IGUAL A QUE A PESSOA INSERIU
        public bool Verify(string senha, string hash)
            => BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}
