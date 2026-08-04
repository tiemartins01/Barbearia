using Barbearia.Core.Domain.ValueObjects;
using Barbearia.Core.DTO;
using Barbearia.Core.Exceptions;
using Barbearia.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Barbearia.Controllers
{
    [ApiController]
    [Route("cadastro")]
    public class NovoUsuarioController: ControllerBase
    {

        private readonly INovoClienteService _service;

        public NovoUsuarioController(INovoClienteService service)
        {
            _service = service;
        }

        [HttpPost]
        [HttpPost("~/api/v1/users")]
        [IgnoreAntiforgeryToken]
        [EnableRateLimiting("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] DTONovoUsuario request)
        {
            
                var resultado = await _service.CadastrarAsync(request.Nome, request.Email, request.Phone, request.CPF, request.Login, request.SenhaR, request.Foto);

                return Created();
        }
    }
}
