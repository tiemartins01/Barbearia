using Barbearia.Core.DTO;
using Barbearia.Core.Excepetion;
using Barbearia.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Barbearia.Controllers
{
    [ApiController]
    [Route("trocar")]
    public class TrocarSenhaController : ControllerBase
    {

        private readonly ITrocaSenhaService _service;

        public TrocarSenhaController(ITrocaSenhaService service)
        {
            _service = service;
        }

        [HttpPost]
        [HttpPost("~/api/v1/password/reset")]
        [IgnoreAntiforgeryToken]
        [EnableRateLimiting("troca-senha")]
        public async Task<IActionResult> TrocarSenha([FromBody] DTOMudarSenha request)
        {
                var resultado = await _service.RealizarTrocaSenha(request.Codigo, request.Email, request.Senha, request.SenhaRepetida);
                return Ok(resultado);
        }
    }
}
