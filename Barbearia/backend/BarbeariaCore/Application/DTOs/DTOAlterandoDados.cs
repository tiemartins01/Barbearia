namespace Barbearia.Core.DTO
{
    public class DTOAlterandoDados
    {
        // Não cria os objetos conforme Email, Phone, Senha, CPF
        //o DTO fica desacoplado do domínio;
        //o domínio continua utilizando seus Value Objects;
        //a validação permanece centralizada nos métodos Criar() dos Value Objects.
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string SenhaAntiga { get; set; } = null!;
        public string NovaSenha { get; set; } = null!;

    }
}
