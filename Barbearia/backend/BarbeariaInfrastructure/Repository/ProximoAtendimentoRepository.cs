using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using BarbeariaCore.Application.Models;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public class ProximoAtendimentoRepository
        : IProximoAtendimentoRepository
    {
        private readonly AppDbContext _context;

        public ProximoAtendimentoRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<DTOProximoAgendamento?>
            InfoProximoAgendamento(
                int idUsuario)
        {
            var agora = DateTime.Now;

            return await _context.Agendamentos
                .AsNoTracking()
                .Where(x =>
                    x.ClienteId == idUsuario &&
                    x.DataAgendamento > agora &&
                    x.Status ==
                        StatusAgendamento.Agendado)
                .OrderBy(x =>
                    x.DataAgendamento)
                .Select(x =>
                    new DTOProximoAgendamento
                    {
                        Horario =
                            x.DataAgendamento
                                .ToString(
                                    "yyyy-MM-ddTHH:mm:ss"),

                        NomeBarbeiro =
                            x.Barbeiro.Usuario.Nome,

                        NomeServico =
                            x.Servico.Nome
                    })
                .FirstOrDefaultAsync();
        }

        public async Task MarcarAgendamento(
            Agendamento agendamento)
        {
            await _context.Agendamentos
                .AddAsync(agendamento);
        }

        public Task<bool> ExisteConflitoAsync(
            int barbeiroId,
            DateTime inicio,
            DateTime fim)
        {
            inicio =
                DateTime.SpecifyKind(
                    inicio,
                    DateTimeKind.Unspecified);

            fim =
                DateTime.SpecifyKind(
                    fim,
                    DateTimeKind.Unspecified);

            return _context.Agendamentos
                .AsNoTracking()
                .AnyAsync(x =>
                    x.BarbeiroId == barbeiroId &&
                    x.Status ==
                        StatusAgendamento.Agendado &&

                    inicio < x.HorarioFim &&
                    fim > x.DataAgendamento);
        }

        public Task<bool> BarbeiroExiste(
            int barbeiroId)
        {
            return _context.Barbeiros
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == barbeiroId &&
                    x.Usuario.Ativado);
        }

        public Task<Servico?> ObterServicoAsync(
            int servicoId)
        {
            return _context.Servicos
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == servicoId &&
                    x.Ativo);
        }

        public async Task<
            IReadOnlyList<PeriodoOcupado>>
            BuscarPeriodosOcupadosAsync(
                int barbeiroId,
                DateOnly data)
        {
            var inicioDia =
                data.ToDateTime(
                    TimeOnly.MinValue);

            var fimDia =
                data.AddDays(1)
                    .ToDateTime(
                        TimeOnly.MinValue);

            return await _context.Agendamentos
                .AsNoTracking()
                .Where(x =>
                    x.BarbeiroId == barbeiroId &&
                    x.Status ==
                        StatusAgendamento.Agendado &&

                    x.DataAgendamento < fimDia &&
                    x.HorarioFim > inicioDia)
                .OrderBy(x =>
                    x.DataAgendamento)
                .Select(x =>
                    new PeriodoOcupado
                    {
                        Inicio =
                            x.DataAgendamento,

                        Fim =
                            x.HorarioFim
                    })
                .ToListAsync();
        }
    }
}