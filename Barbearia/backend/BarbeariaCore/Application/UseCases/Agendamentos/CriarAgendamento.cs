using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Exceptions;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Exceptions;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Exceptions;
using Microsoft.Extensions.Logging;

namespace BarbeariaCore.UseCases.Agendamentos
{
    public sealed class CriarAgendamento
    {

        private readonly IAgendamentoRepository _agendamentos;
        private readonly IBarbeiroRepository _barbeiros;
        private readonly IServicoRepository _servicos;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CriarAgendamento> _logger;

        public CriarAgendamento (IAgendamentoRepository agendamentos, 
            IBarbeiroRepository barbeiros, IServicoRepository servicos, 
            IUnitOfWork uow, ILogger<CriarAgendamento> logger)
        {
            _agendamentos = agendamentos;
            _barbeiros = barbeiros;
            _servicos = servicos;
            _uow = uow;
            _logger = logger;
        }

        public async Task<DTOResposta> ExecutarAsync(
            int idBarbeiro,
            int idUsuario,
            int idServico,
            DateTime horario)
        {
            ValidarIds(idBarbeiro, idUsuario, idServico);

            var agora = DateTime.Now;

            if (!await _barbeiros.ExisteAtivoAsync(idBarbeiro))
                throw new NotFoundException(
                    "BARBER_NOT_FOUND",
                    "Barbeiro não encontrado.");

            var servico = await _servicos.ObterAtivoPorIdAsync(idServico);

            if (servico is null)
                throw new NotFoundException(
                    "SERVICE_NOT_FOUND",
                    "Serviço não encontrado.");

            var fim = horario.AddMinutes(servico.Duracao);

            var existeConflito =
                await _agendamentos.ExisteConflitoAsync(
                    idBarbeiro,
                    horario,
                    fim);

            try
            {
                PoliticaAgenda.GarantirDisponibilidade(existeConflito);
            }
            catch (DomainException ex)
                when (ex.Code == "APPOINTMENT_TIME_CONFLICT")
            {
                throw new ConflictException(
                    "APPOINTMENT_TIME_CONFLICT",
                    "O barbeiro já possui um agendamento ativo neste período.");
            }

            var agendamento = new Agendamento(
                idUsuario,
                idBarbeiro,
                idServico,
                servico.Duracao,
                horario,
                agora);

            await _uow.BeginTransactionAsync();

            try
            {
                await _agendamentos.AdicionarAsync(agendamento);

                await _uow.SaveChangesAsync();

                agendamento.RegistrarCriacao();

                await _uow.SaveChangesAsync();

                await _uow.CommitTransactionAsync();

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

        private static void ValidarIds(
            int idBarbeiro,
            int idUsuario,
            int idServico)
        {
            if (idBarbeiro <= 0)
                throw new ValidationException("BARBER_ID_INVALID", 
                    "O identificador do barbeiro é inválido.");

            if (idUsuario <= 0)
                throw new ValidationException("USER_ID_INVALID", 
                    "O identificador do usuário é inválido.");

            if (idServico <= 0)
                throw new ValidationException("SERVICE_ID_INVALID",
                    "O identificador do serviço é inválido.");
        }

    }
}
