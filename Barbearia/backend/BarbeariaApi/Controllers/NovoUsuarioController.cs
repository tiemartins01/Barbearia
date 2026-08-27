using BarbeariaCore.Application.DTOs;
using BarbeariaCore.UseCases.Autenticacao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("cadastro")]
    public class NovoUsuarioController: ControllerBase
    {

        private readonly CadastrarCliente _service;

        public NovoUsuarioController(CadastrarCliente service)
        {
            _service = service;
        }

        [HttpPost]
        [HttpPost("~/api/v1/users")]
        [IgnoreAntiforgeryToken]
        [EnableRateLimiting("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] DTONovoUsuario request)
        {
            
                var resultado = await _service.ExecutarAsync(request.Nome, 
                    request.Email, request.Phone, request.CPF, request.Login, 
                    request.SenhaR, request.Foto);

                return Created();
        }
    }
}
