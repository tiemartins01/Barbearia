using Barbearia.Core.DTO;
using FluentValidation;

namespace Barbearia.Core.Application.Validation;

public sealed class DTOEnviarEmailValidator : AbstractValidator<DTOEnviarEmail>
{
    public DTOEnviarEmailValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254);
    }
}
