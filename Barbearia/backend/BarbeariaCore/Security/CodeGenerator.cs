using System.Security.Cryptography;

namespace Barbearia.Core.Security
{
    public static class CodeGenerator
    {
        // GERAR O CÓDIGO DE 6 DIGITOS PARA REALIZAR A TROCA DA SENHA
        public static string GerarCod()
        {
            int numero = RandomNumberGenerator.GetInt32(0, 1000000);
            return numero.ToString("D6");
        }
    }
}