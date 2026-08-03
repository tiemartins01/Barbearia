using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Barbearia.Filters;

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
            var argumentType = argument!.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            foreach (var group in result.Errors.GroupBy(x => x.PropertyName))
                errors[group.Key] = group.Select(x => x.ErrorMessage).Distinct().ToArray();
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
