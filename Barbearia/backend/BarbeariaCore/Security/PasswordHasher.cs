namespace Barbearia.Core.Security
{
    public class PasswordHasher
    {
        // GERA O HASH DA SENHA
        public static string Hash(string senha)
            => BCrypt.Net.BCrypt.HashPassword(senha);
        // VERIFICA SE A SENHA NO BANCO É IGUAL A QUE A PESSOA INSERIU
        public static bool Verify(string senha, string hash)
            => BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}
