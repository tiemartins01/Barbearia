using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.DTO;
using Barbearia.Core.Exceptions;
using Barbearia.Core.Interface;
using BarbeariaCore.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Barbearia.Core.Service
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
            var lista = await _repository.BuscarTodosBarbeiros();

            if (lista.Count == 0)
                throw new DomainException("WITHOUT_BARBER","Sem barbeiros cadastrados!");

            return lista;
        }

        // APENAS PARA RETORNAR O HISTÓRICO DOS CLIENTES
        public async Task<List<DTOHistorico>> HistoricoCliente(int idCliente, int page, int pageSize)
        {
            var historico = await _repository.Historico(idCliente,page, pageSize);

            if (historico.Count == 0)
                throw new DomainException("NO_HISTORY","Nenhum serviço realizado!");

            return historico;
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
                IdCliente = horario.Id_cliente,
                IdBarbeiro = horario.Id_barbeiro,
                IdServico = horario.Id_servico,
                Horario = horario.Horario,
                Status = horario.StatusAgendamento
            };
        }

        public async Task<DTOHorarioDetalhes?> InfoHorarioDoCliente(int id, int userId)
        {
            var horario = await _repository.HorarioValidoAsync(id);
            if (horario is null) return null;

            if (horario.Id_cliente != userId)
                throw new DomainException("RESOURCE_ACCESS_DENIED", "Você não possui acesso a este agendamento.");

            return new DTOHorarioDetalhes
            {
                Id = horario.Id,
                IdCliente = horario.Id_cliente,
                IdBarbeiro = horario.Id_barbeiro,
                IdServico = horario.Id_servico,
                Horario = horario.Horario,
                Status = horario.StatusAgendamento
            };
        }

        // ALTERANDO OS DADOS PESSOAIS 
        public async Task AlterandoDados(DTOAlterandoDados dados)
        {
            var usuario = await _repository.GetUsuarioAsync(dados.Id);

            if(usuario == null)
                throw new DomainException("AUTH_INVALID_CREDENTIALS", "Credencial inválida!");

            usuario.AlterarDados(dados.Nome,
            new Email(dados.Email),
            new Phone(dados.Telefone),
            new Cpf(dados.Cpf));

            if (!string.IsNullOrEmpty(dados.NovaSenha))
            {
                if (!usuario.Senha.Verify(dados.SenhaAntiga,_passwordHash) || string.IsNullOrEmpty(dados.SenhaAntiga))
                    throw new DomainException("AUTH_INVALID_CREDENTIALS","Credenciais inválidas!");

                usuario.AlterarSenha(new Senha(dados.NovaSenha));
            }
            await _uow.SaveChangesAsync();
        }

        // REALIZAR A AVALIAÇÃO
        public async Task RealizandoAvaliacaoAsync(DTOAvaliacao avaliacao, int id_cliente)
        {
            await InformacoesFora(avaliacao, id_cliente);

            var nova_avaliacao = new Avaliacoes
                (
                    avaliacao.Id_barbeiro,
                    id_cliente,
                    avaliacao.Id_horario,
                    avaliacao.Nota,
                    avaliacao.Comentario,
                    avaliacao.Horario,
                    avaliacao.Id_servico
                );

            if (!nova_avaliacao.HorarioMenor(avaliacao.Horario))
            {
                _logger.LogWarning("{} tentou avaliar antes de concluir o horário", id_cliente);
                throw new DomainException("ACTION_DENIED", "Dados inválidos!");
            }

            await _repository.RealizarAvaliacaoAsync(nova_avaliacao);
            var horarioParaAtualizar = await _repository.BuscarHorarioParaAtualizarAsync(avaliacao.Id_horario);
            if (horarioParaAtualizar != null)
                horarioParaAtualizar.Avaliado();

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
                throw new DomainException("ACTION_DENIED", "Dados inválidos!");
            }

            if (horario.StatusAgendamento != Enum.StatusAgendamento.Concluido)
            {
                _logger.LogWarning("Status diferente de concluido no id {}", avaliacao.Id);
                throw new DomainException("DIFFERENT_STATUS", "Status indisponível!");
            }

            if (horario.Id_cliente != id_cliente)
            {
                _logger.LogWarning("Id do cliente não é o mesmo de logado! Id: {}", id_cliente);
                throw new DomainException("AUTH_INVALID_CREDENTIALS","Informação inválida!");
            }

            if (horario.Id_barbeiro != avaliacao.Id_barbeiro)
            {
                _logger.LogWarning("Id do barbeiro não é o mesmo do horário! Id: {}", avaliacao.Id_barbeiro);
                throw new DomainException("AUTH_INVALID_CREDENTIALS","Informação inválida!");
            }

            if(horario.Id_servico != avaliacao.Id_servico)
            {
                _logger.LogWarning("Id do serviço não é o mesmo do horário! Id: {}", avaliacao.Id_servico);
                throw new DomainException("AUTH_INVALID_CREDENTIALS","Informação inválida!");
            }
            if (horario.Id != avaliacao.Id_horario)
            {
                _logger.LogWarning("Id do horário não é o mesmo do banco de dados! Id: {}", avaliacao.Id_horario);
                throw new DomainException("AUTH_INVALID_CREDENTIALS", "Informação inválida!");
            }
        }
    }
}
