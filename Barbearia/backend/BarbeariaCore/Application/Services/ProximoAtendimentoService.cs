using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Exceptions;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Exceptions;
using Microsoft.Extensions.Logging;

using ValidationException =
    BarbeariaCore.Exceptions.ValidationException;

namespace BarbeariaCore.Application.Services
{
    public class ProximoAtendimentoService
        : IProximoAtendimentoService
    {
        private readonly
            IProximoAtendimentoRepository _repository;

        private readonly IUnitOfWork _uow;

        private readonly
            ILogger<ProximoAtendimentoService> _logger;

        public ProximoAtendimentoService(
            IProximoAtendimentoRepository repository,
            IUnitOfWork uow,
            ILogger<ProximoAtendimentoService> logger)
        {
            _repository = repository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<DTOProximoAgendamento?>
            ObterProximoAtendimentoAsync(
                int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new ValidationException(
                    "USER_ID_INVALID",
                    "O identificador do usuário é inválido.");
            }

            var agendamento =
                await _repository
                    .InfoProximoAgendamento(
                        idUsuario);

            if (agendamento is null)
            {
                _logger.LogInformation(
                    "Usuário {IdUsuario} não possui agendamentos futuros.",
                    idUsuario);
            }

            return agendamento;
        }

        public async Task<DTOResposta>
            AgendarHorarioAsync(
                int idBarbeiro,
                int idUsuario,
                int idServico,
                DateTime horario)
        {
            ValidarIds(
                idBarbeiro,
                idUsuario,
                idServico);

            var agora = DateTime.Now;

            await ValidarBarbeiroAsync(
                idBarbeiro);

            var servico =
                await ObterServicoAsync(
                    idServico);

            var duracao =
                servico.Duracao;

            await ValidarDisponibilidadeAsync(
                idBarbeiro,
                horario,
                duracao);

            var agendamento =
                new Agendamento(
                    idUsuario,
                    idBarbeiro,
                    idServico,
                    duracao,
                    horario,
                    agora);

            await _uow.BeginTransactionAsync();

            try
            {
                await _repository
                    .MarcarAgendamento(
                        agendamento);

                // Primeiro Save:
                // gera o Id do Agendamento.
                await _uow.SaveChangesAsync();

                // Agora o Id é válido.
                agendamento.RegistrarCriacao();

                // Segundo Save:
                // grava o Domain Event no Outbox.
                await _uow.SaveChangesAsync();

                await _uow
                    .CommitTransactionAsync();

                _logger.LogInformation(
                    "Agendamento criado. Cliente={Cliente} Barbeiro={Barbeiro} Serviço={Servico} Inicio={Inicio} Fim={Fim}",
                    idUsuario,
                    idBarbeiro,
                    idServico,
                    agendamento.DataAgendamento,
                    agendamento.HorarioFim);

                return new DTOResposta
                {
                    Sucesso = true,
                    Mensagem =
                        "Horário agendado com sucesso!"
                };
            }
            catch (
                PersistenceConflictException ex)
                when (
                    ex.Code ==
                    "APPOINTMENT_TIME_CONFLICT")
            {
                await _uow.RollbackAsync();

                _logger.LogWarning(
                    ex,
                    "Conflito ao criar agendamento. Barbeiro={Barbeiro} Inicio={Inicio}",
                    idBarbeiro,
                    horario);

                throw new ConflictException(
                    "APPOINTMENT_TIME_CONFLICT",
                    "O barbeiro já possui um agendamento ativo neste período.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Erro ao gravar agendamento. Cliente={Cliente} Barbeiro={Barbeiro} Horario={Horario}",
                    idUsuario,
                    idBarbeiro,
                    horario);

                throw;
            }
        }

        public async Task<
            IReadOnlyCollection<TimeOnly>>
            ObterHorariosDisponiveisAsync(
                int idBarbeiro,
                int idServico,
                DateOnly data)
        {
            if (idBarbeiro <= 0)
            {
                throw new ValidationException(
                    "BARBER_ID_INVALID",
                    "O identificador do barbeiro é inválido.");
            }

            if (idServico <= 0)
            {
                throw new ValidationException(
                    "SERVICE_ID_INVALID",
                    "O identificador do serviço é inválido.");
            }

            var agora = DateTime.Now;

            PoliticaAgenda.ValidarDataNaoPassada(
                data,
                DateOnly.FromDateTime(agora));

            await ValidarBarbeiroAsync(
                idBarbeiro);

            var servico =
                await ObterServicoAsync(
                    idServico);

            var periodosOcupados =
                await _repository
                    .BuscarPeriodosOcupadosAsync(
                        idBarbeiro,
                        data);

            var grade =
                PoliticaAgenda
                    .GerarGradeHorario();

            var disponiveis =
                new List<TimeOnly>();

            foreach (var horarioGrade in grade)
            {
                var inicio =
                    data.ToDateTime(
                        horarioGrade);

                // Se for hoje, não mostra horários
                // que já passaram.
                if (inicio <= agora)
                    continue;

                if (!PoliticaAgenda
                    .CabeNoExpediente(
                        inicio,
                        servico.Duracao))
                {
                    continue;
                }

                var fim =
                    inicio.AddMinutes(
                        servico.Duracao);

                var existeConflito =
                    periodosOcupados.Any(
                        periodo =>
                            PoliticaAgenda
                                .ExisteSobreposicao(
                                    inicio,
                                    fim,
                                    periodo.Inicio,
                                    periodo.Fim));

                if (!existeConflito)
                {
                    disponiveis.Add(
                        horarioGrade);
                }
            }

            return disponiveis;
        }

        private static void ValidarIds(
            int idBarbeiro,
            int idUsuario,
            int idServico)
        {
            if (idBarbeiro <= 0)
            {
                throw new ValidationException(
                    "BARBER_ID_INVALID",
                    "O identificador do barbeiro é inválido.");
            }

            if (idUsuario <= 0)
            {
                throw new ValidationException(
                    "USER_ID_INVALID",
                    "O identificador do usuário é inválido.");
            }

            if (idServico <= 0)
            {
                throw new ValidationException(
                    "SERVICE_ID_INVALID",
                    "O identificador do serviço é inválido.");
            }
        }

        private async Task
            ValidarBarbeiroAsync(
                int idBarbeiro)
        {
            var existe =
                await _repository
                    .BarbeiroExiste(
                        idBarbeiro);

            if (existe)
                return;

            _logger.LogWarning(
                "Barbeiro inexistente. Id={IdBarbeiro}",
                idBarbeiro);

            throw new NotFoundException(
                "BARBER_NOT_FOUND",
                "Barbeiro não encontrado.");
        }

        private async Task<Servico>
            ObterServicoAsync(
                int idServico)
        {
            var servico =
                await _repository
                    .ObterServicoAsync(
                        idServico);

            if (servico is not null)
                return servico;

            _logger.LogWarning(
                "Serviço inexistente ou inativo. Id={IdServico}",
                idServico);

            throw new NotFoundException(
                "SERVICE_NOT_FOUND",
                "Serviço não encontrado.");
        }

        private async Task
            ValidarDisponibilidadeAsync(
                int idBarbeiro,
                DateTime horario,
                int duracao)
        {
            var fim =
                horario.AddMinutes(
                    duracao);

            var existeConflito =
                await _repository
                    .ExisteConflitoAsync(
                        idBarbeiro,
                        horario,
                        fim);

            try
            {
                PoliticaAgenda
                    .GarantirDisponibilidade(
                        existeConflito);
            }
            catch (DomainException ex)
                when (
                    ex.Code ==
                    "APPOINTMENT_TIME_CONFLICT")
            {
                _logger.LogWarning(
                    "Tentativa de criar agendamento sobreposto. Barbeiro={Barbeiro} Inicio={Inicio} Fim={Fim}",
                    idBarbeiro,
                    horario,
                    fim);

                throw new ConflictException(
                    "APPOINTMENT_TIME_CONFLICT",
                    "O barbeiro já possui um agendamento ativo neste período.");
            }
        }
    }
}