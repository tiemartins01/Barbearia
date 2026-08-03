using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbeariaInfrastructure
{
    public class SmtpSettings
    {
        public bool Enabled { get; set; }
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;

    }
}
