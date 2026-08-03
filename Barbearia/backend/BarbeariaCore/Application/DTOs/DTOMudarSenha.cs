namespace Barbearia.Core.DTO
{
    public class DTOMudarSenha
    {

        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string SenhaRepetida { get; set; } = string.Empty;

    }
}
