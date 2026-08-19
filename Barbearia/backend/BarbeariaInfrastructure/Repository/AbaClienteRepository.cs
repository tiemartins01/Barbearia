using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Infrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public class AbaClienteRepository :IAbaClienteRepository
    {

        private readonly AppDbContext _context;

        public AbaClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        // VAI BUSCAR TODOS OS BARBEIROS ATIVOS

        public  Task<Usuario?> GetUsuarioAsync(int id) => _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<DTOBarbeiro>> BuscarTodosBarbeiros()
        {
            var barbeiros = await _context.Barbeiros
                .AsNoTracking() // APENAS LEITURA, MELHORA O DESEMPENHO DA PESQUISA
                .Where(b => b.Usuario.Role == RolePerson.Barbeiro &&
                            b.Usuario.Ativado)
                .OrderBy(b => b.Usuario.Nome) // ORDENA EM ORDEM ALFABÉTICA
                .Select(b => new
                {
                    b.Id,
                    b.Usuario.Nome,
                    b.Especialidade,

                    NotaMedia = _context.Avaliacoes
                        .Where(a => a.IdBarbeiro == b.Id)
                        .Average(a => (double?)a.Nota) ?? 0,

                    QuantidadeAvaliacoes = _context.Avaliacoes
                        .Count(a => a.IdBarbeiro == b.Id)
                })
                .ToListAsync();

            return barbeiros.Select(x => new DTOBarbeiro
            {
                Id = x.Id,
                Nome = x.Nome,
                Iniciais = GerarIniciais(x.Nome),
                Especialidade = x.Especialidade,
                NotaMedia = x.NotaMedia,
                QuantidadeAvaliacoes = x.QuantidadeAvaliacoes
            }).ToList();
        }

        private static string GerarIniciais(string nome)
        {
            return string.Concat(
                nome.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(p => char.ToUpperInvariant(p[0])));
        }

        // HISTÓRICO DE SERVIÇOS DO CLIENTE, SENDO APENAS OS SERVIÇOS QUE FORAM CONCLUI
        public  Task<List<DTOHistorico>> Historico(int id, int page, int pageSize) =>
            _context.Horarios.AsNoTracking().Where(x => x.Id_cliente == id && (x.StatusAgendamento == StatusAgendamento.Avaliado || x.StatusAgendamento == StatusAgendamento.Concluido))
                .OrderByDescending(x => x.Horario)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DTOHistorico
                {
                    Id = x.Id,
                    NomeServico = x.Servicos.Nome,
                    NomeBarbeiro = x.Barbeiro.Usuario.Nome,
                    ValorServico = x.Servicos.Preco,
                    Data = x.Horario,
                    PodeAvaliar = x.StatusAgendamento == StatusAgendamento.Concluido
                }).ToListAsync();
        
        // TODOS OS DADOS PESSOAIS DO CLIENTE
        public async Task<DTODadosPessoais> DadosPessoais(int id)
        {
            var dados = await _context.Usuarios
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select( x => new
            {
                x.Id,
                x.Nome,
                Iniciais = GerarIniciais(x.Nome),
                Email = x.Email.ToString(),
                Telefone = x.Phone.Telefone,
                Cpf = x.CPF.Numero,
                QtdCortes = _context.Horarios.Count(a =>
                    a.Id_cliente == x.Id &&
                    (a.StatusAgendamento == StatusAgendamento.Concluido ||
                        a.StatusAgendamento == StatusAgendamento.Avaliado))
                }).SingleOrDefaultAsync();


            if (dados is null)
                throw new InvalidOperationException($"Usuário {id} não encontrado ao consultar dados pessoais.");

            return new DTODadosPessoais
            {
                Id = dados.Id,
                Nome = dados.Nome,
                Iniciais = GerarIniciais(dados.Nome),
                Email = dados.Email,
                Qtdcortes = dados.QtdCortes,
                Telefone = dados.Telefone,
                Cpf = dados.Cpf
            };
        }
        public  Task<Horarios?> BuscarHorarioParaAtualizarAsync(int id) => _context.Horarios.FirstOrDefaultAsync(x => x.Id == id && StatusAgendamento.Avaliado != x.StatusAgendamento);
        public  Task<Horarios?>HorarioValidoAsync(int id) =>  _context.Horarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id );
        public async Task RealizarAvaliacaoAsync(Avaliacoes avaliacao)
        {
            await _context.Avaliacoes.AddAsync(avaliacao);
        }
    }
}
