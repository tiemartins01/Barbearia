using BarbeariaCore.Application.DTOs;
using BarbeariaCore.UseCases.Autenticacao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("trocar")]
    public class TrocarSenhaController : ControllerBase
    {

        private readonly RedefinirSenha _service;

        public TrocarSenhaController(RedefinirSenha service)
        {
            _service = service;
        }

        [HttpPost]
        [HttpPost("~/api/v1/password/reset")]
        [IgnoreAntiforgeryToken]
        [EnableRateLimiting("troca-senha")]
        public async Task<IActionResult> TrocarSenha([FromBody] DTOMudarSenha request)
        {
                var resultado = await _service.ExecutarAsync(request.Codigo, 
                    request.Email, request.Senha, request.SenhaRepetida);
                return Ok(resultado);
        }
    }
}
