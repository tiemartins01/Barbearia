using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BarbeariaApi.Controllers
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
