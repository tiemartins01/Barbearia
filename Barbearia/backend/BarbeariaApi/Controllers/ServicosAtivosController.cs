using Barbearia.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Barbearia.Controllers
{
    [ApiController]
    [Route("servicos")]
    public class ServicosAtivosController : ControllerBase
    {

        private readonly IServicosService _service;

        public ServicosAtivosController(IServicosService service)
        {
            _service = service;
        }
        [Authorize(Roles = "Cliente")]
        [HttpGet("ativos")]
        [HttpGet("~/api/v1/services")]
        public async Task<IActionResult> ServicosAtivos()
        {
            var servicos = await _service.CarregarServicosAtivos();

            return Ok(servicos);
        }

    }
}
