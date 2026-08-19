namespace BarbeariaCore.Domain.Entities
{
    public sealed class Barbeiro
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Especialidade { get; set; } = string.Empty;
        public Usuario Usuario { get; set; } = null!; 
        public ICollection<Avaliacoes> Avaliacoes { get; set; } = new List<Avaliacoes>();
    }
}
