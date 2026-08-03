namespace Barbearia.Core.Interface
{
    public interface IEmailEsqueciSenhaService
    {
        Task EnviarEmailAsync(string email);

    }
}
