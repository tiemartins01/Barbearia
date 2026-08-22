using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Domain.Policies;
using BarbeariaCore.Domain.ValueObjects;
using BarbeariaCore.Exceptions;
using Microsoft.Extensions.Logging;
using AuthenticationException = BarbeariaCore.Exceptions.AuthenticationException;
using ForbiddenException = BarbeariaCore.Exceptions.ForbiddenException;
using ValidationException = BarbeariaCore.Exceptions.ValidationException;

namespace BarbeariaCore.Application.Services
{
    public class AbaClienteService : IAbaClienteService
    {
        private readonly IAbaClienteRepository _repository;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AbaClienteService> _logger;
        private readonly IPasswordHash _passwordHash;

        public AbaClienteService(IAbaClienteRepository repository, IUnitOfWork uow, ILogger<AbaClienteService> logger, IPasswordHash password)
        {
            _repository = repository;
            _uow = uow;
            _logger = logger;
            _passwordHash = password;
        }

        // APENAS PARA RETORNAR OS BARBEIROS CADASTRADOS E ATIVOS
        public async Task<List<DTOBarbeiro>> BuscarBarbeiros()
        {
            return await _repository.BuscarTodosBarbeiros();
        }

        // APENAS PARA RETORNAR O HISTÓRICO DOS CLIENTES
        public async Task<List<DTOHistorico>> HistoricoCliente(int idCliente, int page, int pageSize)
        {
            return await _repository.Historico(idCliente, page, pageSize);
        }

        // APENAS PARA RETORNAR OS DADOS PESSOAIS DO CLIENTE
        public async Task<DTODadosPessoais> DadosPessoaisAsync(int idCliente)
        {
            return await _repository.DadosPessoais(idCliente);
        }

        // PEGA OS HORÁRIOS VÁLIDOS
        public async Task<DTOHorarioDetalhes?> InfoHorario(int id)
        {
            var horario = await _repository.HorarioValidoAsync(id);
            if (horario is null)
                return null;

            return new DTOHorarioDetalhes
            {
                Id = horario.Id,
                IdCliente = horario.ClienteId,
                IdBarbeiro = horario.BarbeiroId,
                IdServico = horario.ServicoId,
                Horario = horario.DataAgendamento,
                Status = horario.Status
            };
        }

        public async Task<DTOHorarioDetalhes?> InfoHorarioDoCliente(int id, int userId)
        {
            var horario = await _repository.HorarioValidoAsync(id);
            if (horario is null) return null;

            if (horario.ClienteId != userId)
                throw new ForbiddenException("RESOURCE_ACCESS_DENIED", "Você não possui acesso a este agendamento.");

            return new DTOHorarioDetalhes
            {
                Id = horario.Id,
                IdCliente = horario.ClienteId,
                IdBarbeiro = horario.BarbeiroId,
                IdServico = horario.ServicoId,
                Horario = horario.DataAgendamento,
                Status = horario.Status
            };
        }

        // ALTERANDO OS DADOS PESSOAIS 
        public async Task AlterandoDados(DTOAlterandoDados dados)
        {
            var usuario = await _repository.GetUsuarioAsync(dados.Id);

            if (usuario is null)
                throw new AuthenticationException("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");

            usuario.AlterarDados(dados.Nome,
            new Email(dados.Email),
            new Telefone(dados.Telefone),
            new Cpf(dados.Cpf));


            if (!string.IsNullOrEmpty(dados.NovaSenha))
            {
                PoliticaSenha.Validar(dados.NovaSenha);

                if (string.IsNullOrEmpty(dados.SenhaAntiga) || !_passwordHash.Verify(dados.SenhaAntiga, usuario.Senha.Hash))
                    throw new AuthenticationException("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");

                var senhaHash = _passwordHash.Hash(dados.NovaSenha);

                var senhaDominio = Senha.DeHash(senhaHash);

                usuario.AlterarSenha(senhaDominio);
            }
            await _uow.SaveChangesAsync();
        }

        // REALIZAR A AVALIAÇÃO
        public async Task RealizandoAvaliacaoAsync(DTOAvaliacao avaliacao, int id_cliente)
        {
            await InformacoesFora(avaliacao, id_cliente);

            var nova_avaliacao = new Avaliacao
                (
                    avaliacao.Id_barbeiro,
                    id_cliente,
                    avaliacao.Id_horario,
                    avaliacao.Nota,
                    avaliacao.Comentario,
                    avaliacao.Horario,
                    avaliacao.Id_servico
                );

            //if (!nova_avaliacao.HorarioMenor(avaliacao.Horario))
            //{
            //    _logger.LogWarning("{} tentou avaliar antes de concluir o horário", id_cliente);
            //    throw new ConflictException("EVALUATION_NOT_ALLOWED", "A avaliação não pode ser realizada neste momento.");
            //}

            await _repository.RealizarAvaliacaoAsync(nova_avaliacao);
            var horarioParaAtualizar = await _repository.BuscarHorarioParaAtualizarAsync(avaliacao.Id_horario);
            if (horarioParaAtualizar != null)
                horarioParaAtualizar.MarcarComoAvaliado();

            await _uow.SaveChangesAsync();

            _logger.LogInformation("Avaliacao correta feita no horário de id: {}", avaliacao.Id_horario);
        }

        // VERIFICA QUAL INFORMAÇÃO NÃO ESTÁ DE ACORDO
        private async Task InformacoesFora(DTOAvaliacao avaliacao, int id_cliente)
        {
            var horario = await _repository.HorarioValidoAsync(avaliacao.Id_horario);

            if (horario == null)
            {
                _logger.LogWarning("Informações inexistente!");
                throw new NotFoundException("APPOINTMENT_NOT_FOUND", "Agendamento não encontrado.");
            }

            if (horario.Status != StatusAgendamento.Concluido)
            {
                _logger.LogWarning("Status diferente de concluido no id {}", avaliacao.Id);
                throw new ConflictException("APPOINTMENT_NOT_COMPLETED", "Status indisponível!");
            }

            if (horario.ClienteId != id_cliente)
            {
                _logger.LogWarning("Id do cliente não é o mesmo de logado! Id: {}", id_cliente);
                throw new ForbiddenException("RESOURCE_ACCESS_DENIED","Você não possui acesso a este agendamento.");
            }

            if (horario.BarbeiroId != avaliacao.Id_barbeiro)
            {
                _logger.LogWarning("Id do barbeiro não é o mesmo do horário! Id: {}", avaliacao.Id_barbeiro);
                throw new ValidationException("APPOINTMENT_BARBER_MISMATCH","O barbeiro informado não corresponde ao agendamento.");
            }

            if (horario.ServicoId != avaliacao.Id_servico)
            {
                _logger.LogWarning("Id do serviço não é o mesmo do horário! Id: {}", avaliacao.Id_servico);
                throw new ValidationException("APPOINTMENT_SERVICE_MISMATCH","O serviço informado não corresponde ao agendamento.");
            }
        }
    }
}
