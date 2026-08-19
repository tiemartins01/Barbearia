using BarbeariaCore.Domain.Exceptions;
namespace BarbeariaCore.Domain.ValueObjects
{
    public sealed class Cpf // sealed para não ter herança
    {
        public string Numero { get; private set; } = string.Empty; // alteração só nessa classe
        private Cpf() { } // Entity Framework

        public Cpf(string numero) 
        {
           Numero = ApenasDigitos(numero);

            if (!IsValid(Numero))
                throw new DomainException("USER_INVALID_CPF", "CPF inválido");           
        }

        // só aceita número
        private static string ApenasDigitos(string? valor) =>
            new((valor ?? string.Empty).Where(char.IsDigit).ToArray());

        private bool IsValid(string cpf)
        {
            if (cpf.Length != 11 || cpf.All(digito => digito == cpf[0]))
                return false;

            var soma = 0;
            for (var i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
        }

        public override string ToString() => Numero;
    }
}
