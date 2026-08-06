using Barbearia.Core.Domain.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenTelemetry.Metrics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace Barbearia.Filters;
// Valida os objetos recebidos pelo Controller antes de executar a action

//Requisição chega
//    ↓
//Model Binding cria o DTO
//    ↓
//RequestValidationFilter
//    ↓
//Procura um validator para o DTO
//    ↓
//Executa as validações
//    ↓
//Se houver erro: retorna 400
//Se estiver válido: executa o Controller

public sealed class RequestValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public RequestValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var argument in context.ActionArguments.Values.Where(x => x is not null))
        {
            var argumentType = argument!.GetType(); // Pega o tipo real do argumento atual
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType); // Monta dinamicante o tipo do validator

            if (_serviceProvider.GetService(validatorType) is not IValidator validator) // validator recebe um dos DTO dentro de Core.Validation 
                continue;

            var validationContext = new ValidationContext<object>(argument);// Cria o contexto que será enviado ao Fluent
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted); // Verifica as regras dentro do Dto do Core.Validation
            // context.HttpContext.RequestAborted -> representa um CancellationToken

            // Salva os erros pelo nome da propriedade para depois aparecerem
            foreach (var group in result.Errors.GroupBy(x => x.PropertyName))
                errors[group.Key] = group.Select(x => x.ErrorMessage).Distinct().ToArray();

            //Exemplo de erros:

            //Senha → Senha é obrigatória.
            //Senha → Senha precisa ter 8 caracteres.
            //Login → Login é obrigatório.
        }

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(new
            {
                sucesso = false,
                codigo = "VALIDATION_ERROR",
                mensagem = "Um ou mais campos são inválidos.",
                erros = errors,
                traceId = context.HttpContext.TraceIdentifier
            });
            return;
        }

        await next();
    }
}
// Exemplo: 
//{
//  "sucesso": false,
//  "codigo": "VALIDATION_ERROR",
//  "mensagem": "Um ou mais campos são inválidos.",
//  "erros": {
//    "Nome": [
//      "Login é obrigatório."
//    ],
//    "Senha": [
//      "Senha é obrigatória."
//    ]
//  },
//  "traceId": "f9c8d4bca51e4a82a9a7d2237d5de814"
//}
