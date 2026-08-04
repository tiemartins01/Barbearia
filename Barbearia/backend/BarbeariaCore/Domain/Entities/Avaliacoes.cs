using Barbearia.Core.Exceptions;

namespace Barbearia.Core.Domain.Entities
{
    public sealed class Avaliacoes
    {
        #region Coluna das tabelas
        public int Id { get; private set; }
        public int IdBarbeiro { get; private set; }
        public int IdCliente { get; private set; }
        public int IdHorario { get; private set; }
        public int Nota { get; private set; }
        public string? Comentario { get; private set; } = string.Empty;
        public DateTime Horario { get; private set; }
        public int IdServico { get; private set; }
        public Barbeiro BarbeiroF { get; private set; } = null!;
        #endregion
        private Avaliacoes(){ }
        public Avaliacoes(int id_barbeiro, int id_cliente, int id_horario, int nota, string? comentario, DateTime horario, int id_servico)
        {
            if (id_barbeiro <= 0 || id_cliente <= 0 || id_horario <= 0 || id_servico <= 0)
                throw new DomainException("REVIEW_INVALID_REFERENCE", "Os dados relacionados à avaliação são inválidos.");

            if (nota is < 1 or > 5)
                throw new DomainException("REVIEW_INVALID_SCORE", "A nota deve estar entre 1 e 5.");

            var comentarioNormalizado = comentario?.Trim(); 

            if (comentarioNormalizado?.Length > 128)
                throw new DomainException("REVIEW_COMMENT_TOO_LONG", "O comentário deve possuir no máximo 128 caracteres.");

            IdBarbeiro = id_barbeiro;
            IdCliente = id_cliente;
            IdHorario = id_horario;
            Nota = nota;
            Comentario = comentarioNormalizado;
            Horario = DateTime.SpecifyKind(horario, DateTimeKind.Unspecified);
            IdServico = id_servico;
        }

        public bool HorarioMenor(DateTimeOffset horarioC) => horarioC.LocalDateTime < DateTime.Now;
    }
}
