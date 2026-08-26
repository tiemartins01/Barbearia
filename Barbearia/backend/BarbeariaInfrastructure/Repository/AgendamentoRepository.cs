using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Repository
{
    public sealed class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly AppDbContext _context;

        public AgendamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Agendamento?> ObterPorIdAsync(int agendamentoId) =>
            _context.Agendamentos.FirstOrDefaultAsync(x => x.Id == agendamentoId);

        public Task<bool> ExisteConflitoAsync(
            int barbeiroId,
            DateTime inicio,
            DateTime fim)
        {
            inicio = DateTime.SpecifyKind(inicio, DateTimeKind.Unspecified);
            fim = DateTime.SpecifyKind(fim, DateTimeKind.Unspecified);

            return _context.Agendamentos
                .AsNoTracking()
                .AnyAsync(x =>
                    x.BarbeiroId == barbeiroId &&
                    x.Status == StatusAgendamento.Agendado &&
                    inicio < x.HorarioFim &&
                    fim > x.DataAgendamento);
        }

        public async Task AdicionarAsync(Agendamento agendamento) =>
            await _context.Agendamentos.AddAsync(agendamento);

        public Task AtualizarAsync(Agendamento agendamento)
        {
            _context.Agendamentos.Update(agendamento);
            return Task.CompletedTask;
        }
    }
}
