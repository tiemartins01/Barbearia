using Barbearia.Core.Exceptions;
using System.Text.Json;

namespace Barbearia.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status400BadRequest);
        }
        catch (AuthenticationException ex)
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status401Unauthorized);
        }
        catch (ForbiddenException ex)
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status403Forbidden);
        }
        catch (ConflictException ex)
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status409Conflict);
        }
        catch (AppException ex)
        {
            // Segurança para alguma exceção de aplicação
            // que ainda não tenha um mapeamento específico.
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro não tratado em {Metodo} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            await WriteResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR",
                "Ocorreu um erro interno inesperado.");
        }
    }

    private async Task HandleAppExceptionAsync(
        HttpContext context,
        AppException exception,
        int statusCode)
    {
        _logger.LogWarning(
            exception,
            "Erro de aplicação {Codigo} em {Metodo} {Path}. TraceId: {TraceId}",
            exception.Code,
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);

        await WriteResponseAsync(
            context,
            statusCode,
            exception.Code,
            exception.Message);
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var response = new
        {
            sucesso = false,
            codigo = code,
            mensagem = message,
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}
