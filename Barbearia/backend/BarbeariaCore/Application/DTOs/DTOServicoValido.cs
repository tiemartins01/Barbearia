namespace BarbeariaCore.Application.DTOs
{
    public class DTOServicoValido
    {
        public int Id { get; set; }
        public int Id_cliente { get; set; }
        public int Id_barbeiro { get; set; }
        public int Id_servico { get; set; }
        public DateTime Horario { get; set; }
    }
}
