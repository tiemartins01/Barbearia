namespace Barbearia.Core.DTO
{
    public class DTOHistorico
    {
        public int Id { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public string NomeBarbeiro {  get; set; } = string.Empty;
        public decimal ValorServico { get; set; }
        public DateTime Data {  get; set; }
        public bool PodeAvaliar { get; set; }
    }
}

