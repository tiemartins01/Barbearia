using Barbearia.Core.Exceptions;
using BarbeariaCore.Application.Interfaces;

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

        public static Senha Criar(string senha, IPasswordHash _hash)
        {
            ValidarSenhaEmTextoPlano(senha);

            ArgumentNullException.ThrowIfNull(_hash);


            var hash = _hash.Hash(senha);

            return new Senha(hash);
        }

        public bool Verify(string senha, IPasswordHash _hash) 
        {
            if (string.IsNullOrWhiteSpace(senha))
                return false;

            return _hash.Verify(senha, SenhaHash);
        }

        private static void ValidarSenhaEmTextoPlano(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new DomainException("USER_INVALID_passwordHash", "Senha em branco.");
            if (senha.Length < 6)
                throw new DomainException("USER_INVALID_passwordHash", "A senha deve ter no mínimo 6 caracteres.");
        }

        public override string ToString() => "********";
    }
}
