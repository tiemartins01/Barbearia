using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.Entities
{
    public sealed class Avaliacao
    {
        #region Coluna das tabelas
        public int Id { get; private set; }
        public int BarbeiroId { get; private set; }
        public int ClieteId { get; private set; }
        public int AgendamentoId { get; private set; }
        public int Nota { get; private set; }
        public string? Comentario { get; private set; } = string.Empty;
        public DateTime DataAtendimento { get; private set; }
        public int ServicoId { get; private set; }
        public Barbeiro Barbeiro { get; private set; } = null!;
        #endregion
        private Avaliacao(){ }
        public Avaliacao(int barbeiroId, int clienteId, int agendamentoId, int nota, string? comentario, DateTime dataAgendamento, int servicoId)
        {
            if (barbeiroId <= 0 || clienteId <= 0 || agendamentoId <= 0 || servicoId <= 0)
                throw new DomainException("REVIEW_INVALID_REFERENCE", "Os dados relacionados à avaliação são inválidos.");

            if (nota is < 1 or > 5)
                throw new DomainException("REVIEW_INVALID_SCORE", "A nota deve estar entre 1 e 5.");

            var comentarioNormalizado = comentario?.Trim(); 

            if (comentarioNormalizado?.Length > 128)
                throw new DomainException("REVIEW_COMMENT_TOO_LONG", "O comentário deve possuir no máximo 128 caracteres.");

            BarbeiroId = barbeiroId;
            ClieteId = clienteId;
            AgendamentoId = agendamentoId;
            Nota = nota;
            Comentario = comentarioNormalizado;
            DataAtendimento = DateTime.SpecifyKind(dataAgendamento, DateTimeKind.Unspecified);
            ServicoId = servicoId;
        }
        // Quem decide se pode ou não ser avaliado é agendamento
    }
}
