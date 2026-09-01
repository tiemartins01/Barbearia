using BarbeariaCore.Application.Abstractions;
using BarbeariaCore.Application.DTOs;
using BarbeariaCore.UseCases.Agendamentos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ValidationException = BarbeariaCore.Exceptions.ValidationException;

namespace BarbeariaApi.Controllers
{
    [ApiController]
    [Route("agendamento")]
    [Authorize(Policy = "ClientOnly")]
    public class ProximoAgendamentoController : ControllerBase
    {
        private readonly ConsultarProximoAgendamento _service;
        private readonly ConsultarHorariosDisponiveis _disponiveis;
        private readonly CriarAgendamento _criar;
        private readonly ICurrentUser _user;
        private readonly IIdempotencyService _idempotency;

        public ProximoAgendamentoController(
            ConsultarProximoAgendamento service,
            ICurrentUser user,
            IIdempotencyService idempotency,
            ConsultarHorariosDisponiveis disponiveis,
            CriarAgendamento criar)
        {
            _service = service;
            _user = user;
            _idempotency = idempotency;
            _disponiveis = disponiveis;
            _criar = criar;
        }

        [HttpGet("proximo")]
        [HttpGet("~/api/v1/appointments/next")]
        public async Task<IActionResult> ProximoAgendamento(CancellationToken cancellationToken)
        {
            var proximo = await _service.ExecutarAsync(_user.UserId, cancellationToken);
            return Ok(proximo);
        }

        [HttpGet("horarioslivres")]
        [HttpGet("~/api/v1/appointments/available-slots")]
        public async Task<IActionResult> ConsultarHorariosLivres(
     [FromQuery] int id_barbeiro,
     [FromQuery] int id_servico,
     [FromQuery] DateOnly data,
     CancellationToken cancellationToken)
        {
            var horarios = await _disponiveis.ExecutarAsync(
                id_barbeiro,
                id_servico,
                data,
                cancellationToken);

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
                () => _criar.ExecutarAsync(
                    request.BarbeiroId,
                    _user.UserId,
                    request.ServicoId,
                    request.Horario,cancellationToken),
                cancellationToken);

            Response.Headers["Idempotency-Replayed"] = result.Replayed ? "true" : "false";
            return Ok(result.Value);
        }
        
    }
}
