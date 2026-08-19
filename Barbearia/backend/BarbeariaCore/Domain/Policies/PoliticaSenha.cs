using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.Policies
{
    public  static class PoliticaSenha
    {

        public static void Validar(string senha)
        {
            if (string.IsNullOrEmpty(senha))
            {
                throw new DomainException(
                    "USER_INVALID_PASSWORD",
                    "Senha em branco.");
            }

            if (senha.Length < 6)
            {
                throw new DomainException(
                    "USER_INVALID_PASSWORD",
                    "A senha deve ter no mínimo 6 caracteres.");
            }
        }

    }
}
