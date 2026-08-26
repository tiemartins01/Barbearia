using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Queries;
using BarbeariaCore.Domain.Enum;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Queries
{
    public sealed class DadosPessoaisQuery : IDadosPessoaisQuery
    {
        private readonly AppDbContext _context;

        public DadosPessoaisQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DTODadosPessoais?> ConsultarAsync(int clienteId)
        {
            var dados = await _context.Usuarios
                .AsNoTracking()
                .Where(x => x.Id == clienteId)
                .Select(x => new
                {
                    x.Id,
                    x.Nome,
                    Email = x.Email.Valor,
                    Telefone = x.Numero.Valor,
                    Cpf = x.CPF.Valor,
                    QtdCortes = _context.Agendamentos.Count(a =>
                        a.ClienteId == x.Id &&
                        (a.Status == StatusAgendamento.Concluido ||
                         a.Status == StatusAgendamento.Avaliado))
                })
                .SingleOrDefaultAsync();

            if (dados is null)
                return null;

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

        private static string GerarIniciais(string nome) =>
            string.Concat(
                nome.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(p => char.ToUpperInvariant(p[0])));
    }
}
