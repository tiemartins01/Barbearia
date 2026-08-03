using Barbearia.Core.Application.Abstractions;
using Barbearia.Core.DTO;
using Barbearia.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Data;
using System.Security.Claims;

namespace Barbearia.Controllers
{
    [ApiController]
    [Route("cliente")]
    [Authorize(Roles = "Cliente")]
    public class AbaClienteController : ControllerBase
    {

        private readonly IAbaClienteService _service;
        private readonly ICurrentUser _user;

        public AbaClienteController(IAbaClienteService service, ICurrentUser user)
        {
            _service = service;
            _user = user;
        }
        [HttpGet("barbeiros")]
        [HttpGet("~/api/v1/barbers")]
        public async Task<IActionResult> BarbeirosCadastrados() => Ok(await _service.BuscarBarbeiros());


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

            var resultado = await _service.HistoricoCliente(_user.UserId, page, pageSize);

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
        public async Task<IActionResult> DadosPessais() => Ok(await _service.DadosPessoaisAsync(_user.UserId));

        [HttpPost("infoHorario")]
        [HttpGet("~/api/v1/appointments/{idHorario:int}")]
        public async Task<IActionResult> informacoesHorario([FromBody] DTOInfoHorario request, [FromRoute] int? idHorario = null) => Ok(await _service.InfoHorarioDoCliente(idHorario ?? request.IdHorario, _user.UserId));

        [HttpPost("alterarDados")]
        [HttpPatch("~/api/v1/users/me")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarDadosPessoais([FromBody] DTOAlterandoDados request) 
        {
            request.Id = _user.UserId;

            await _service.AlterandoDados(request);

            return NoContent();
        }

        [HttpPost("avaliacao")]
        [HttpPost("~/api/v1/reviews")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avaliacao([FromBody] DTOAvaliacao request)
        {
            await _service.RealizandoAvaliacaoAsync(request, _user.UserId); 

            return Ok(request);
        }
    }
}
