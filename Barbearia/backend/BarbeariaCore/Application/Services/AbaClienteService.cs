using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Application.Interfaces.Services;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.Exceptions;
using Microsoft.Extensions.Logging;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;
using ForbiddenException = BarbeariaCore.Exceptions.ForbiddenException;

namespace BarbeariaCore.Application.Services
{
    public sealed class AbaClienteService : IAbaClienteService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IAgendamentoRepository _agendamentos;
        private readonly IAvaliacaoRepository _avaliacoes;
        private readonly IBarbeirosQuery _barbeirosQuery;
        private readonly IHistoricoClienteQuery _historicoQuery;
        private readonly IDadosPessoaisQuery _dadosPessoaisQuery;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AbaClienteService> _logger;
        private readonly IPasswordHash _passwordHash;

        public AbaClienteService(
            IUsuarioRepository usuarios,
            IAgendamentoRepository agendamentos,
            IAvaliacaoRepository avaliacoes,
            IBarbeirosQuery barbeirosQuery,
            IHistoricoClienteQuery historicoQuery,
            IDadosPessoaisQuery dadosPessoaisQuery,
            IUnitOfWork uow,
            ILogger<AbaClienteService> logger,
            IPasswordHash passwordHash)
        {
            _usuarios = usuarios;
            _agendamentos = agendamentos;
            _avaliacoes = avaliacoes;
            _barbeirosQuery = barbeirosQuery;
            _historicoQuery = historicoQuery;
            _dadosPessoaisQuery = dadosPessoaisQuery;
            _uow = uow;
            _logger = logger;
            _passwordHash = passwordHash;
        }

        public async Task<List<DTOBarbeiro>> BuscarBarbeiros()
        {
            var barbeiros = await _barbeirosQuery.ListarAtivosAsync();
            return barbeiros.ToList();
        }

        public async Task<List<DTOHistorico>> HistoricoCliente(
            int idCliente,
            int page,
            int pageSize)
        {
            var historico =
                await _historicoQuery.ConsultarAsync(
                    idCliente,
                    page,
                    pageSize);

            return historico.ToList();
        }

        public Task<DTODadosPessoais?> DadosPessoaisAsync(int idCliente) =>
            _dadosPessoaisQuery.ConsultarAsync(idCliente);

        public async Task<DTOHorarioDetalhes?> InfoHorario(int id)
        {
            var agendamento = await _agendamentos.ObterPorIdAsync(id);

            if (agendamento is null)
                return null;

            return MapearHorario(agendamento);
        }

        public async Task<DTOHorarioDetalhes?> InfoHorarioDoCliente(
            int id,
            int userId)
        {
            var agendamento = await _agendamentos.ObterPorIdAsync(id);

            if (agendamento is null)
                return null;

            if (agendamento.ClienteId != userId)
                throw new ForbiddenException(
                    "RESOURCE_ACCESS_DENIED",
                    "Você não possui acesso a este agendamento.");

            return MapearHorario(agendamento);
        }

        public async Task AlterandoDados(DTOAlterandoDados dados)
        {
            var usuario = await _usuarios.ObterPorIdAsync(dados.Id);

            if (usuario is null)
                throw new AuthenticationException(
                    "AUTH_INVALID_CREDENTIALS",
                    "Credencial inválida!");

            usuario.AlterarDados(
                dados.Nome,
                new Email(dados.Email),
                new Telefone(dados.Telefone),
                new Cpf(dados.Cpf));

            if (!string.IsNullOrWhiteSpace(dados.NovaSenha))
            {
                PoliticaSenha.Validar(dados.NovaSenha);

                if (string.IsNullOrWhiteSpace(dados.SenhaAntiga) ||
                    !_passwordHash.Verify(
                        dados.SenhaAntiga,
                        usuario.Senha.Hash))
                {
                    throw new AuthenticationException(
                        "AUTH_INVALID_CREDENTIALS",
                        "Credencial inválida!");
                }

                var senhaHash = _passwordHash.Hash(dados.NovaSenha);
                var senhaDominio = Senha.DeHash(senhaHash);

                usuario.AlterarSenhaPerfil(senhaDominio);
            }

            await _usuarios.AtualizarAsync(usuario);
            await _uow.SaveChangesAsync();
        }

        public async Task RealizandoAvaliacaoAsync(
            DTOAvaliacao avaliacao,
            int idCliente)
        {
            var agendamento =
                await _agendamentos.ObterPorIdAsync(
                    avaliacao.AgendamentoId);

            if (agendamento is null)
                throw new NotFoundException(
                    "APPOINTMENT_NOT_FOUND",
                    "Agendamento não encontrado.");

            if (agendamento.ClienteId != idCliente)
                throw new ForbiddenException(
                    "RESOURCE_ACCESS_DENIED",
                    "Você não possui acesso a este agendamento.");

            if (await _avaliacoes.ExisteParaAgendamentoAsync(agendamento.Id))
                throw new ConflictException(
                    "REVIEW_ALREADY_EXISTS",
                    "Este atendimento já possui avaliação.");

            // Autoridade do status fica no Aggregate.
            agendamento.MarcarComoAvaliado();

            var novaAvaliacao = new Avaliacao(
                agendamento.BarbeiroId,
                agendamento.ClienteId,
                agendamento.Id,
                avaliacao.Nota,
                avaliacao.Comentario,
                agendamento.DataAgendamento,
                agendamento.ServicoId);

            await _avaliacoes.AdicionarAsync(novaAvaliacao);
            await _uow.SaveChangesAsync();

            _logger.LogInformation(
                "Avaliação realizada. Cliente={ClienteId} Agendamento={AgendamentoId}",
                idCliente,
                agendamento.Id);
        }

        private static DTOHorarioDetalhes MapearHorario(Agendamento agendamento) =>
            new()
            {
                Id = agendamento.Id,
                IdCliente = agendamento.ClienteId,
                IdBarbeiro = agendamento.BarbeiroId,
                IdServico = agendamento.ServicoId,
                Horario = agendamento.DataAgendamento,
                Status = agendamento.Status
            };
    }
}
