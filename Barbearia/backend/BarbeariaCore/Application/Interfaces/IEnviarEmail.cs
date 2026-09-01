namespace BarbeariaCore.Application.Interfaces
{
    public interface IEnviarEmail 
    {

        Task EnviarEmailAsync(string destinatario, string assunto, string mensagem, CancellationToken cancellationToken);

    }
}
