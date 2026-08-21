using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.ValueObjects
{
    public sealed record class Telefone
    {
        public string Valor { get; private set; }

        private Telefone() { } // ENTITY FRAMEWORK
        public Telefone(string telefone) 
        {
            var numeros = new string((telefone ?? string.Empty).Where(char.IsDigit).ToArray());

            if (numeros.Length != 11)
                throw new DomainException("USER_INVALID_PHONE", "Telefone inválido!");

            Valor = numeros;
        }

        public override string ToString() => Valor;

    }
}
