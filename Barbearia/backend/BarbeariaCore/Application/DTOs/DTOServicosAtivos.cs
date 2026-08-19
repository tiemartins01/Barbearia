namespace BarbeariaCore.Application.DTOs
{
    public class DTOServicosAtivos
    {
        public int Id { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public decimal Preco { get; set; }   
        public int Duracao { get; set; }

    }
}
