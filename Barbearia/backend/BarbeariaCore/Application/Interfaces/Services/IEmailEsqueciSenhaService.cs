namespace BarbeariaCore.Application.Interfaces.Services
{
    public interface IEmailEsqueciSenhaService
    {
        Task EnviarEmailAsync(string email);
    }
}
