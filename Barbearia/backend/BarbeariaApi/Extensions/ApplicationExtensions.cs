using BarbeariaCore.UseCases.Agendamentos;
using BarbeariaCore.UseCases.Autenticacao;
using BarbeariaCore.UseCases.Cliente;
using BarbeariaCore.UseCases.Security;
using BarbeariaCore.UseCases.Servicos;

namespace BarbeariaApi.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddBarbeariaApplication(
            this IServiceCollection services)
        {
            // Cliente
            services.AddScoped<ListarBarbeiros>();
            services.AddScoped<ConsultarHistoricoCliente>();
            services.AddScoped<ConsultarDadosPessoais>();
            services.AddScoped<ConsultarAgendamento>();
            services.AddScoped<ConsultarAgendamentoDoCliente>();
            services.AddScoped<AlterarDadosPessoais>();
            services.AddScoped<AvaliarAtendimento>();

            // Agendamentos
            services.AddScoped<ConsultarProximoAgendamento>();
            services.AddScoped<CriarAgendamento>();
            services.AddScoped<ConsultarHorariosDisponiveis>();

            // Autenticação
            services.AddScoped<RealizarLogin>();
            services.AddScoped<CadastrarCliente>();
            services.AddScoped<SolicitarRecuperacaoSenha>();
            services.AddScoped<RedefinirSenha>();

            // Serviços
            services.AddScoped<ListarServicosAtivos>();

            // Refresh / sessões
            services.AddScoped<RenovarToken>();
            services.AddScoped<RevogarToken>();
            services.AddScoped<ListarSessoes>();
            services.AddScoped<RevogarTodasSessoes>();
            services.AddScoped<RevogarSessao>();

            return services;
        }
    }
}
