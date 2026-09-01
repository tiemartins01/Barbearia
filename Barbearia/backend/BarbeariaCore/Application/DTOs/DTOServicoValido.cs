namespace BarbeariaCore.Application.DTOs
{
    public class DTOServicoValido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }

        public int BarbeiroId { get; set; }

        public int ServicoId { get; set; }
        public DateTime Horario { get; set; }
    }
}
