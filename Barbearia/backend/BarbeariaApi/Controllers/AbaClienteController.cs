using BarbeariaCore.Application.Abstractions;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces.Repositories;
using BarbeariaCore.UseCases.Cliente;
using BarbeariaInfrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("cliente")]
    [Authorize(Policy = "ClientOnly")]
    public class AbaClienteController : ControllerBase
    {

        private readonly ListarBarbeiros _listarBarbeiros;
        private readonly ConsultarHistoricoCliente _consultarHistorico;
        private readonly ConsultarDadosPessoais _consultarDados;
        private readonly ConsultarAgendamentoDoCliente _consultarAgendamento;
        private readonly AlterarDadosPessoais _alterarDados;
        private readonly AvaliarAtendimento _avaliarAtendimento;
        private readonly ICurrentUser _usuario;

        public AbaClienteController(
            ListarBarbeiros listarBarbeiros,
            ConsultarHistoricoCliente consultarHistorico,
            ConsultarDadosPessoais consultarDados,
            ConsultarAgendamentoDoCliente consultarAgendamento,
            AlterarDadosPessoais alterarDados,
            AvaliarAtendimento avaliarAtendimento,
            ICurrentUser usuario)
        {
            _listarBarbeiros = listarBarbeiros;
            _consultarHistorico = consultarHistorico;
            _consultarDados = consultarDados;
            _consultarAgendamento = consultarAgendamento;
            _alterarDados = alterarDados;
            _avaliarAtendimento = avaliarAtendimento;
            _usuario = usuario;
        }

        [HttpGet("barbeiros")]
        [HttpGet("~/api/v1/barbers")]
        public async Task<IActionResult> BarbeirosCadastrados() => Ok(await _listarBarbeiros.ExecutarAsync());


        [HttpGet("historico")]
        [HttpGet("~/api/v1/appointments/history")]
        public async Task<IActionResult> ObterHistorico(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            if (page < 1)
            {
                return BadRequest(new
                {
                    erro = "A página deve ser maior ou igual a 1."
                });
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new
                {
                    erro = "O tamanho da página deve estar entre 1 e 100."
                });
            }

            var resultado = await _consultarHistorico.ExecutarAsync(_usuario.UserId, page, pageSize);

            return Ok(resultado);
        }
        // _user.UserId NA REAL FAZ:
        //_user
        //↓
        //HttpCurrentUser
        // ↓
        //IHttpContextAccessor
        //  ↓
        //HttpContext atual
        //  ↓
        //HttpContext.User
        //  ↓
        //ClaimTypes.NameIdentifier
        //  ↓
        //conversão para int
        //  ↓
        //retorna o ID

        [HttpGet("dados")]
        [HttpGet("~/api/v1/users/me")]
        public async Task<IActionResult> DadosPessais() => Ok(await _consultarDados.ExecutarAsync(_usuario.UserId));

        [HttpPost("infoHorario")]
        [HttpGet("~/api/v1/appointments/{idHorario:int}")]
        public async Task<IActionResult> informacoesHorario([FromBody] DTOInfoHorario request, 
            [FromRoute] int? idHorario = null)
            => Ok(await _consultarAgendamento.ExecutarAsync(idHorario ?? request.IdHorario, _usuario.UserId));

        [HttpPost("alterarDados")]
        [HttpPatch("~/api/v1/users/me")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarDadosPessoais([FromBody] DTOAlterandoDados request) 
        {
            request.Id = _usuario.UserId;

            await _alterarDados.ExecutarAsync(request);

            return NoContent();
        }

        [HttpPost("avaliacao")]
        [HttpPost("~/api/v1/reviews")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avaliacao([FromBody] DTOAvaliacao request)
        {
            await _avaliarAtendimento.ExecutarAsync(request, _usuario.UserId); 

            return Ok(request);
        }
    }
}
