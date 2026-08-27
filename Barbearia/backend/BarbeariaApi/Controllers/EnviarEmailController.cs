using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Exceptions;
using BarbeariaCore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using BarbeariaCore.UseCases.Autenticacao;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("envioe")]
    public class EnviarEmailController : ControllerBase
    {

        private readonly SolicitarRecuperacaoSenha _service;

        public EnviarEmailController(SolicitarRecuperacaoSenha service)
        {
            _service = service;
        }

        [HttpPost]
        [HttpPost("~/api/v1/password/recovery")]
        [IgnoreAntiforgeryToken]
        [EnableRateLimiting("recuperacao-senha")]
        public async Task<IActionResult> EnviarEmail([FromBody] DTOEnviarEmail request)
        {
                                  
            await _service.ExecutarAsync(request.Email);

            return Ok(new
            {
                Sucesso = true,
                Message = "E-mail enviado com sucesso"
            });
        }

    }
}
