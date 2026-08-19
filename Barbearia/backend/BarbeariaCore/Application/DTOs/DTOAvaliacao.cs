namespace BarbeariaCore.Application.DTOs
{
    public class DTOAvaliacao
    {

        public int Id { get; set; }
        public int Id_barbeiro { get; set; }
        public int Id_horario { get; set; }
        public int Id_servico { get; set; }
        public int Nota {  get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime Horario { get; set; }
    }
}
