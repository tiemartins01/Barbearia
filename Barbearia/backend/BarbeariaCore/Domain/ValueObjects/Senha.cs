using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.ValueObjects
{
    public sealed record class Senha
    {
        public string Hash { get; private set; } = string.Empty;

        private Senha() { } // ENTITY FRAMEWORK
        private Senha(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                throw new DomainException(
                   "USER_INVALID_PASSWORD",
                   "Hash da senha inválido.");
            }

            Hash = hash;
        }
        
        public static Senha DeHash(string hash)
        {
            return new Senha(hash);
        }

        public override string ToString() => "********";
    }
}
