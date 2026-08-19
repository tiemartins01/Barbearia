namespace BarbeariaCore.Application.Interfaces
{
    public interface IEmailEsqueciSenhaService
    {
        Task EnviarEmailAsync(string email);

    }
}
