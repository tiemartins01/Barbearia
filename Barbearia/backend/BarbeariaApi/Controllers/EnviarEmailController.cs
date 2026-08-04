using Barbearia.Core.DTO;
using Barbearia.Core.Exceptions;
using Barbearia.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Barbearia.Controllers
{
    [ApiController]
    [Route("envioe")]
    public class EnviarEmailController : ControllerBase
    {

        private readonly IEmailEsqueciSenhaService _service;

        public EnviarEmailController(IEmailEsqueciSenhaService service)
        {
            _service = service;
        }

        [HttpPost]
        [HttpPost("~/api/v1/password/recovery")]
        [IgnoreAntiforgeryToken]
        [EnableRateLimiting("recuperacao-senha")]
        public async Task<IActionResult> EnviarEmail([FromBody] DTOEnviarEmail request)
        {
                                  
            await _service.EnviarEmailAsync(request.Email);

            return Ok(new
            {
                Sucesso = true,
                Message = "E-mail enviado com sucesso"
            });
        }

    }
}
