using System.Net.Mail;
using Barbearia.Core.Exceptions;

namespace Barbearia.Core.Domain.ValueObjects
{
    public sealed class Email
    {
        public string EmailPessoa { get; private set; } = string.Empty;

        private Email () { } // Entity Framework
        public Email(string emailPessoa) 
        {
            var emailNormalizado = (emailPessoa ?? string.Empty).Trim().ToLowerInvariant();

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

            EmailPessoa = emailNormalizado;
        }

        public override string ToString() => EmailPessoa;
    }
}
