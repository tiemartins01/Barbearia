using BarbeariaCore.Domain.Entities;
using BarbeariaCore.Domain.Enum;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Infrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace BarbeariaInfrastructure.Repository
{
    public class ProximoAtendimentoRepository : IProximoAtendimentoRepository
    {

        private readonly AppDbContext _context;

        public ProximoAtendimentoRepository(AppDbContext context)
        {
            _context = context;
        }
        // PEGA O PRÓXIMO AGENDAMENTO QUE NÃO FOI CANCELADO OU CONCLUIDO
        public async Task<DTOProximoAgendamento?> InfoProximoAgendamento(int id)
        {
            var agora = DateTime.Now;
            return await _context.Horarios
                .AsNoTracking()
                .Where(x => x.Id_cliente == id && x.Horario > agora && x.StatusAgendamento == StatusAgendamento.Agendado)
                .OrderBy(x => x.Horario)
                .Select(x => new DTOProximoAgendamento
                {
                    Horario = x.Horario.ToString("yyyy-MM-ddTHH:mm:ss"),
                    NomeBarbeiro = x.Barbeiro.Usuario.Nome,
                    NomeServico = x.Servicos.Nome
                })
                .FirstOrDefaultAsync();
        }
        // ADICIONANDO O NOVO AGENDAMENTO
        public async Task MarcarAgendamento(Horarios horario)
        {
            await _context.Horarios.AddAsync(horario);
        }
        // VERIFICA SE O BARBEIRO NAQUELE HORÁRIO ESTÁ DISPONÍVEL
        public Task<bool> DisponibilidadeHorario(DateTime horario, int id_barbeiro)
        {
            horario = DateTime.SpecifyKind(horario, DateTimeKind.Unspecified);

            return _context.Horarios.AsNoTracking().AnyAsync(x =>
                x.Id_barbeiro == id_barbeiro &&
                x.Horario == horario &&
                x.StatusAgendamento == StatusAgendamento.Agendado);
        }
        // ESSE E DO SERVIÇO, É FEITO PARA QUE UMA PESSOA TENTE DE OUTRA FORMA ADICIONAR UM HORÁRIO COM BARBEIRO OU SERVIÇO EXISTENTE
        // EXEMPLO COM O POSTMAN, TENTA ADICIONAR NÃO PELO APLICATIVO
        public  Task<bool> BarbeiroExiste(int id_barbeiro) => _context.Barbeiros.AsNoTracking().AnyAsync(x => x.Id == id_barbeiro && x.Usuario.Ativado);
        public  Task<bool> ServicoExiste(int id_servico) => _context.Servicos.AsNoTracking().AnyAsync(x => x.Id == id_servico && x.Ativo);

        // BUSCA OS HORÁRIOS OCUPADOS PARA QUE NO FRONT NÃO APAREÇA DISPONÍVEL PARA QUE OUTRA PESSOA TENTE AGENDAR O HORÁRIO
        public async Task<List<TimeOnly>> BuscarHorariosOcupadosAsync(int idBarbeiro, DateOnly data)
        {
            var inicio = data.ToDateTime(TimeOnly.MinValue);
            var fim = data.ToDateTime(TimeOnly.MaxValue);

            return await _context.Horarios
                .AsNoTracking()
                .Where(x => x.Id_barbeiro == idBarbeiro
                         && x.Horario >= inicio
                         && x.Horario <= fim)
                .OrderBy(x => x.Horario)
                .Select(x => new TimeOnly(x.Horario.Hour, x.Horario.Minute)) // TimeOnly.FromDateTime(x.Horario)
                .ToListAsync();
        }
    }
}
