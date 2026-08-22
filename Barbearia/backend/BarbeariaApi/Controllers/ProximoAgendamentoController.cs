using BarbeariaCore.Application.Abstractions;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BarbeariaCore.Domain.Exceptions;
using ValidationException = BarbeariaCore.Exceptions.ValidationException;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("agendamento")]
    [Authorize(Roles = "Cliente")]
    public class ProximoAgendamentoController : ControllerBase
    {
        private readonly IProximoAtendimentoService _service;
        private readonly ICurrentUser _user;
        private readonly IIdempotencyService _idempotency;

        public ProximoAgendamentoController(
            IProximoAtendimentoService service,
            ICurrentUser user,
            IIdempotencyService idempotency)
        {
            _service = service;
            _user = user;
            _idempotency = idempotency;
        }

        [HttpGet("proximo")]
        [HttpGet("~/api/v1/appointments/next")]
        public async Task<IActionResult> ProximoAgendamento() => Ok(await _service.ObterProximoAtendimentoAsync(_user.UserId));

        [HttpGet("horarioslivres")]
        [HttpGet("~/api/v1/appointments/available-slots")]
        public async Task<IActionResult> ConsultarHorariosLivres(
     [FromQuery] int id_barbeiro,
     [FromQuery] int id_servico,
     [FromQuery] DateOnly data)
        {
            var horarios = await _service.ObterHorariosDisponiveisAsync(
                id_barbeiro,
                id_servico,
                data);

            return Ok(horarios);
        }

        [HttpPost("marcar")]
        [HttpPost("~/api/v1/appointments")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RealizarAgendamento(
            [FromBody] DTOMarcarHorario request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new ValidationException(
                    "IDEMPOTENCY_KEY_REQUIRED",
                    "O header Idempotency-Key é obrigatório para criar um agendamento.");

            var serializedRequest = JsonSerializer.Serialize(request);
            var requestHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(serializedRequest)));

            var result = await _idempotency.ExecuteAsync(
                idempotencyKey,
                _user.UserId,
                "POST:/api/v1/appointments",
                requestHash,
                () => _service.AgendarHorarioAsync(
                    request.Id_barbeiro,
                    _user.UserId,
                    request.Id_servico,
                    request.horario),
                cancellationToken);

            Response.Headers["Idempotency-Replayed"] = result.Replayed ? "true" : "false";
            return Ok(result.Value);
        }
        
    }
}
