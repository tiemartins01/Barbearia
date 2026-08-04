using BarbeariaInfrastructure;
using Barbearia.Core.DTO;
using Barbearia.Core.Exceptions;
using Barbearia.Core.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Barbearia.Core.Service
{
    public class EnviarEmail : IEnviarEmail
    {
        private readonly SmtpSettings _settings;

        //private readonly IConfiguration _configuration;

        public EnviarEmail(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task EnviarEmailAsync(string destinatario,string assunto,string mensagem)
        {
            if (!_settings.Enabled)
            {
                throw new InvalidOperationException(
                    "O envio de e-mail não está habilitado.");
            }


            using var smtp = new SmtpClient(_settings.Host)
            {
                Port = _settings.Port,
                Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail,
                _settings.FromName),
                Subject = assunto,
                Body = mensagem,
                IsBodyHtml = true
            };

            mail.To.Add(destinatario);

            await smtp.SendMailAsync(mail);
        }

    }
}
