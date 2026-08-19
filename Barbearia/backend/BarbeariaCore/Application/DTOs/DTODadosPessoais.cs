namespace BarbeariaCore.Application.DTOs
{
    public class DTODadosPessoais
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Iniciais { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Qtdcortes { get; set; }
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
    }
}
