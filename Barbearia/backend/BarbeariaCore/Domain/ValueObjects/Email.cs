using System.Net.Mail;
using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.ValueObjects
{
    public sealed record class Email
    {
        public string Valor { get; init; } = string.Empty;

        private Email () { } // Entity Framework
        public Email(string valor) 
        {
            var emailNormalizado = (valor ?? string.Empty).Trim().ToLowerInvariant();

            try
            {
                var endereco = new MailAddress(emailNormalizado); // tenta interceptar esse texto como um email válido
                if (!string.Equals(endereco.Address, emailNormalizado, StringComparison.OrdinalIgnoreCase)) // uma formatação pré na entrada de dados
                    throw new FormatException();
            }
            catch (FormatException)
            {
                throw new DomainException("USER_INVALID_EMAIL", "E-mail inválido.");
            }

            Valor = emailNormalizado;
        }

        public override string ToString() => Valor;
    }
}
