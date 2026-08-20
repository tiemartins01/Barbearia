using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.Entities
{
    public sealed class Barbeiro
    {
        public int Id { get; private set; }
        public int UsuarioId { get; private set; }
        public string Especialidade { get; private set; } = string.Empty;
        public Usuario Usuario { get; private set; } = null!; 
        public ICollection<Avaliacoes> Avaliacoes { get; private set; } = new List<Avaliacoes>();

        private Barbeiro() { }

        public Barbeiro (int usuarioId , string especialidade)
        {
            if (usuarioId <= 0)
                throw new DomainException("USER_INVALID_BARBER", "Barbeiro inválido.");

            UsuarioId = usuarioId;
            Especialidade = NormalizarEspecialidade(especialidade);

        }

        private static string NormalizarEspecialidade(string especialidade)
        {
            if (string.IsNullOrWhiteSpace(especialidade))
                throw new DomainException("SPECIALTY_INVALID", "Especialidade é obrigatória.");

            return especialidade.Trim().ToLowerInvariant();
        }

        public void AlterarEspecialidade(string especialidade)
        {
            Especialidade = NormalizarEspecialidade(especialidade);
        }
    }
}
