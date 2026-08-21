using BarbeariaCore.Exceptions;
using System.Text.Json;
using BarbeariaCore.Domain.Exceptions;


namespace Barbearia.Middleware;
// Middleware responsável por capturar exceções que aconteceram durante a requisição.
// Evita o try e catch em todos os controllers.
// Registra o detalhe técnico da exceção

//Controller ou Service lança uma exceção
//        ↓
//ErrorHandlingMiddleware captura
//        ↓
//Transforma a exceção em resposta HTTP
//        ↓
//Retorna um JSON padronizado

//throw new DomainException("Usuário não encontrado.");
//Pode virar:

//HTTP 400 Bad Request
//{
//  "sucesso": false,
//  "mensagem": "Usuário não encontrado.",
//  "traceId": "abc123"
//}

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
        } // Criado vários tipos de erros para que todos nao retornem exceções que não são planejadas, como erro no sistema.
        catch (ValidationException ex)
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status400BadRequest);
        }
        catch (AuthenticationException ex) // Se a pessoa está ou não autorizada
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status401Unauthorized);
        }
        catch (ForbiddenException ex) // Usuário existe mas está acessando uma área que não pode
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status403Forbidden);
        }
        catch (ConflictException ex) // Informações duplicadas
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status409Conflict);
        }
        catch (NotFoundException ex)
        {
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status404NotFound);
        }
        catch (AppException ex) // Engloba o restante
        {
            // Segurança para alguma exceção de aplicação
            // que ainda não tenha um mapeamento específico.
            await HandleAppExceptionAsync(
                context,
                ex,
                StatusCodes.Status400BadRequest);
        }
        catch (DomainException ex) // Engloba o restante
        {
            _logger.LogWarning(
        ex,
        "Erro de domínio {Codigo} em {Metodo} {Path}. TraceId: {TraceId}",
        ex.Code,
        context.Request.Method,
        context.Request.Path,
        context.TraceIdentifier);

            await WriteResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                ex.Code,
                ex.Message);
        }
        catch (Exception ex) // Erro interno
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
    { // Lança log com o problema
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
        // Limpa e formata a informação
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        // Todos os erros vão vir nesse padrão
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
