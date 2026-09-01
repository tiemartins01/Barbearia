using BarbeariaCore.Application.Interfaces;
using System.Security.Cryptography;

namespace BarbeariaCore.Security
{
    public sealed class CodigoRecuperacaoGenerator : ICodigoRecuperacaoGenerator
    {
        // GERAR O CÓDIGO DE 6 DIGITOS PARA REALIZAR A TROCA DA SENHA
        public string Gerar()
        {
            var numero = RandomNumberGenerator.GetInt32(0, 1000000);
            return numero.ToString("D6");
        }
    }
}