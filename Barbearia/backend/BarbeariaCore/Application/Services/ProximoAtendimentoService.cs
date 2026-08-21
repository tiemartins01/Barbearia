using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Exceptions;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Exceptions;
using Microsoft.Extensions.Logging;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;
using ForbiddenException = BarbeariaCore.Exceptions.ForbiddenException;
using ValidationException = BarbeariaCore.Exceptions.ValidationException;
namespace BarbeariaCore.Application.Services
{
    public class ProximoAtendimentoService : IProximoAtendimentoService
    {
        private readonly IProximoAtendimentoRepository _repository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ProximoAtendimentoService> _logger;

        public ProximoAtendimentoService(
            IProximoAtendimentoRepository repository,
            IUnitOfWork uow,
            ILogger<ProximoAtendimentoService> logger)
        {
            _repository = repository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<DTOProximoAgendamento> ObterProximoAtendimentoAsync(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                _logger.LogWarning(
                    "Tentativa de buscar próximo atendimento com id inválido. Id={IdUsuario}",
                    idUsuario);

                throw new ValidationException("USER_ID_INVALID","O identificador do usuário é inválido.");
            }

            var agendamento = await _repository.InfoProximoAgendamento(idUsuario);

            if (agendamento is null)
            {
                _logger.LogInformation(
                    "Usuário {IdUsuario} não possui agendamentos futuros.",
                    idUsuario);

                return null;
            }

            return agendamento;
        }

        public async Task<DTOResposta> AgendarHorarioAsync(
            int idBarbeiro,
            int idUsuario,
            int idServico,
            DateTime horario)
        {
            var agora = DateTime.Now;

            ValidarParametros(idUsuario, idBarbeiro, idServico);

            if (horario <= agora)
            {
                _logger.LogWarning(
                    "Tentativa de agendar horário passado. Cliente={Cliente} Horario={Horario}",
                    idUsuario,
                    horario);

                throw new ValidationException("APPOINTMENT_DATE_INVALID","O horário do agendamento deve estar no futuro.");
            }

            await ValidarBarbeiroAsync(idBarbeiro);

            await ValidarServicoAsync(idServico);

            await ValidarDisponibilidadeAsync(idBarbeiro, horario);            

            var agendamento = new Agendamento(
                idUsuario,
                idBarbeiro,
                idServico,
                horario,
                agora);

            await _uow.BeginTransactionAsync();

            try
            {
                await _repository.MarcarAgendamento(agendamento);

                await _uow.SaveChangesAsync();

                agendamento.RegistrarCriacao();

                await _uow.SaveChangesAsync();

                await _uow.CommitTransactionAsync();

                _logger.LogInformation(
                    "Agendamento criado. Cliente={Cliente} Barbeiro={Barbeiro} Serviço={Servico} Horario={Horario}",
                    idUsuario,
                    idBarbeiro,
                    idServico,
                    horario);

                return new DTOResposta
                {
                    Sucesso = true,
                    Mensagem = "Horário agendado com sucesso!"
                };
            }
            catch (PersistenceConflictException ex)
            when (ex.Code == "APPOINTMENT_TIME_CONFLICT")
            {
                await _uow.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Erro ao gravar agendamento. Cliente={Cliente} Barbeiro={Barbeiro} Horario={Horario}",
                    idUsuario,
                    idBarbeiro,
                    horario);

                throw new ConflictException(
            "APPOINTMENT_TIME_CONFLICT",
            "O barbeiro já possui um agendamento ativo neste horário.");
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

        public async Task<IReadOnlyCollection<TimeOnly>> ObterHorariosDisponiveisAsync(
            int idBarbeiro,
            DateOnly data)
        {
            if (idBarbeiro <= 0)
                throw new ValidationException("BARBER_ID_INVALID",
    "O identificador do barbeiro é inválido.");

            if (data < DateOnly.FromDateTime(DateTime.Now))
                throw new ValidationException(
    "APPOINTMENT_DATE_INVALID",
    "A data informada não pode estar no passado.");

            await ValidarBarbeiroAsync(idBarbeiro);

            var ocupados = await _repository.BuscarHorariosOcupadosAsync(
                idBarbeiro,
                data);

            var horariosOcupados = ocupados
                .Select(x => new TimeOnly(x.Hour, x.Minute))
                .ToHashSet();

            var horariosDisponiveis = new List<TimeOnly>();

            var horarioAtual = new TimeOnly(8, 0);
            var horarioFinal = new TimeOnly(18, 0);

            while (horarioAtual < horarioFinal)
            {
                if (!horariosOcupados.Contains(horarioAtual))
                    horariosDisponiveis.Add(horarioAtual);

                horarioAtual = horarioAtual.AddMinutes(30);
            }

            return horariosDisponiveis;
        }

        private static void ValidarParametros(
            int idUsuario,
            int idBarbeiro,
            int idServico)
        {
            if (idUsuario <= 0)
                throw new ValidationException("USER_ID_INVALID", "Dados inválidos!");

            if (idBarbeiro <= 0)
                throw new ValidationException("BARBER_ID_INVALID", "Dados inválidos!");

            if (idServico <= 0)
                throw new ValidationException("SERVICE_ID_INVALID", "Dados inválidos!");
        }

        private async Task ValidarBarbeiroAsync(int idBarbeiro)
        {
            var existe = await _repository.BarbeiroExiste(idBarbeiro);

            if (existe)
                return;

            _logger.LogWarning(
                "Barbeiro inexistente. Id={IdBarbeiro}",
                idBarbeiro);

            throw new NotFoundException("BARBER_NOT_FOUND",
    "Barbeiro não encontrado.");
        }

        private async Task ValidarServicoAsync(int idServico)
        {
            var existe = await _repository.ServicoExiste(idServico);

            if (existe)
                return;

            _logger.LogWarning(
                "Serviço inexistente. Id={IdServico}",
                idServico);

            throw new NotFoundException("SERVICE_NOT_FOUND", "Serviço não encontrado.");
        }

        private async Task ValidarDisponibilidadeAsync(
            int idBarbeiro,
            DateTime horario)
        {
            var ocupado = await _repository.DisponibilidadeHorario(
                horario,
                idBarbeiro);

            if (!ocupado)
                return;

            _logger.LogWarning(
                "Tentativa de agendar horário ocupado. Barbeiro={Barbeiro} Horario={Horario}",
                idBarbeiro,
                horario);

            throw new ConflictException("APPOINTMENT_TIME_CONFLICT", "O barbeiro já possui um agendamento ativo neste horário.");
        }
    }
}