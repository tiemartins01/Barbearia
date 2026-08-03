namespace Barbearia.Core.Interface
{
    public interface IEnviarEmail 
    {

        Task EnviarEmailAsync(string destinatario, string assunto, string mensagem);

    }
}
