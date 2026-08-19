namespace BarbeariaCore.Application.DTOs
{
    public class DTOBarbeiro
    {

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Iniciais {  get; set; } = string.Empty; 
        public string Especialidade { get; set; } = string.Empty;
        public double NotaMedia {  get; set; }
        public int QuantidadeAvaliacoes { get; set; }

    }
}
