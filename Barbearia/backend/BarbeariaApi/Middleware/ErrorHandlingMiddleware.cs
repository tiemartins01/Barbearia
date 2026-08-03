using Barbearia.Core.Excepetion;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Barbearia.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            var statusCode = MapStatusCode(ex.Code);

            _logger.LogWarning(
                ex,
                "Erro de domínio {ErrorCode} em {Method} {Path}. TraceId: {TraceId}",
                ex.Code,
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            await WriteProblemDetailsAsync(context, statusCode, ex.Code, ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Requisição inválida em {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "INVALID_REQUEST",
                "A requisição enviada é inválida.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro não tratado em {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR",
                "Ocorreu um erro interno inesperado.");
        }
    }

    private static int MapStatusCode(string code)
    {
        if (code.Contains("ALREADY", StringComparison.OrdinalIgnoreCase) ||
            code.Contains("CONFLICT", StringComparison.OrdinalIgnoreCase) ||
            code is "DIFFERENT_STATUS" or "IDEMPOTENCY_IN_PROGRESS")
        {
            return StatusCodes.Status409Conflict;
        }

        if (code.StartsWith("AUTH_", StringComparison.OrdinalIgnoreCase) ||
            code is "INVALID_REFRESH")
        {
            return StatusCodes.Status401Unauthorized;
        }

        if (code is "ACTION_DENIED" or "RESOURCE_ACCESS_DENIED")
        {
            return StatusCodes.Status403Forbidden;
        }

        if (code is "SESSION_NOT_FOUND" ||
            code.StartsWith("NO_", StringComparison.OrdinalIgnoreCase) ||
            code.StartsWith("WITHOUT_", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status404NotFound;
        }

        return StatusCodes.Status400BadRequest;
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string errorCode,
        string detail)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json; charset=utf-8";

        var problem = new ProblemDetails
        {
            Type = $"https://api.barbearia/errors/{errorCode.ToLowerInvariant().Replace('_', '-')}",
            Title = GetTitle(statusCode),
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["errorCode"] = errorCode;
        problem.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Requisição inválida",
        StatusCodes.Status401Unauthorized => "Não autenticado",
        StatusCodes.Status403Forbidden => "Acesso negado",
        StatusCodes.Status404NotFound => "Recurso não encontrado",
        StatusCodes.Status409Conflict => "Conflito de negócio",
        _ => "Erro interno"
    };
}
