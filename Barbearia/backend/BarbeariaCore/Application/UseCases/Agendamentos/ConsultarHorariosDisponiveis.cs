using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Exceptions;

namespace BarbeariaCore.UseCases.Agendamentos
{
    public sealed class ConsultarHorariosDisponiveis
    {
        private readonly IBarbeiroRepository _barbeiros;
        private readonly IServicoRepository _servicos;
        private readonly IAgendaDisponibilidadeQuery _agendaQuery;

        public ConsultarHorariosDisponiveis(IBarbeiroRepository barbeiros, 
            IServicoRepository servicos, IAgendaDisponibilidadeQuery agendaQuery)
        {
            _barbeiros = barbeiros;
            _servicos = servicos;
            _agendaQuery = agendaQuery;
        }

        public async Task<IReadOnlyCollection<TimeOnly>> ExecutarAsync(
            int idBarbeiro,
            int idServico,
            DateOnly data)
        {
            if (idBarbeiro <= 0)
                throw new ValidationException(
                    "BARBER_ID_INVALID",
                    "O identificador do barbeiro é inválido.");

            if (idServico <= 0)
                throw new ValidationException(
                    "SERVICE_ID_INVALID",
                    "O identificador do serviço é inválido.");

            var agora = DateTime.Now;

            PoliticaAgenda.ValidarDataNaoPassada(
                data,
                DateOnly.FromDateTime(agora));

            if (!await _barbeiros.ExisteAtivoAsync(idBarbeiro))
                throw new NotFoundException(
                    "BARBER_NOT_FOUND",
                    "Barbeiro não encontrado.");

            var servico = await _servicos.ObterAtivoPorIdAsync(idServico);

            if (servico is null)
                throw new NotFoundException(
                    "SERVICE_NOT_FOUND",
                    "Serviço não encontrado.");

            var periodosOcupados =
                await _agendaQuery.BuscarPeriodosOcupadosAsync(
                    idBarbeiro,
                    data);

            var disponiveis = new List<TimeOnly>();

            foreach (var horarioGrade in PoliticaAgenda.GerarGradeHorario())
            {
                var inicio = data.ToDateTime(horarioGrade);

                if (inicio <= agora)
                    continue;

                if (!PoliticaAgenda.CabeNoExpediente(inicio, servico.Duracao))
                    continue;

                var fim = inicio.AddMinutes(servico.Duracao);

                var conflito = periodosOcupados.Any(periodo =>
                    PoliticaAgenda.ExisteSobreposicao(
                        inicio,
                        fim,
                        periodo.Inicio,
                        periodo.Fim));

                if (!conflito)
                    disponiveis.Add(horarioGrade);
            }

            return disponiveis;
        }

    }
}
