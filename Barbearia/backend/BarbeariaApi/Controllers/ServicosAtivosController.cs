using BarbeariaCore.UseCases.Servicos;
using BarbeariaInfrastructure.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("servicos")]
    public class ServicosAtivosController : ControllerBase
    {

        private readonly ListarServicosAtivos _service;

        public ServicosAtivosController(ListarServicosAtivos service)
        {
            _service = service;
        }
        [Authorize(Policy = "ClientOnly")]
        [HttpGet("ativos")]
        [HttpGet("~/api/v1/services")]
        public async Task<IActionResult> ServicosAtivos()
        {
            var servicos = await _service.ExecutarAsync();

            return Ok(servicos);
        }

    }
}
