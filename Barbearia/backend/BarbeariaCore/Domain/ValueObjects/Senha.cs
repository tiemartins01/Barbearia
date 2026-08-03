using Barbearia.Core.Excepetion;

namespace Barbearia.Core.Domain.ValueObjects
{
    public class Senha
    {
        public string SenhaHash {  get; private set; }

        public Senha() { } // ENTITY FRAMEWORK
        public Senha(string hash)
        {
            SenhaHash = hash;
        }

        public static Senha Criar(string senha)
        {
            ValidarSenhaEmTextoPlano(senha);

            var hash = Security.PasswordHasher.Hash(senha);

            return new Senha(hash);
        }

        public bool Verify(string senha) 
        {
            if (string.IsNullOrWhiteSpace(senha))
                return false;

            return Security.PasswordHasher.Verify(senha, SenhaHash);
        }

        private static void ValidarSenhaEmTextoPlano(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new DomainException("USER_INVALID_PASSWORD", "Senha em branco.");
            if (senha.Length < 6)
                throw new DomainException("USER_INVALID_PASSWORD", "A senha deve ter no mínimo 6 caracteres.");
        }

        public override string ToString() => "********";
    }
}
