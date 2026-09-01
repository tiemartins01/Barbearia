namespace BarbeariaCore.Application.DTOs
{
    public sealed class DTONovoUsuario
    {
        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string Cpf { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public string? Foto { get; set; } = string.Empty;

    }
}
