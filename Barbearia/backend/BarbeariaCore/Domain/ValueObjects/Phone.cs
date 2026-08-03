using Barbearia.Core.Excepetion;

namespace Barbearia.Core.Domain.ValueObjects
{
    public sealed class Phone
    {
        public string Telefone { get; private set; }

        private Phone() { } // ENTITY FRAMEWORK
        public Phone(string telefone) 
        {
            var numeros = new string((telefone ?? string.Empty).Where(char.IsDigit).ToArray());

            if (numeros.Length != 11)
                throw new DomainException("USER_INVALID_PHONE", "Telefone inválido!");

            Telefone = numeros;
        }

        public override string ToString() => Telefone;

    }
}
