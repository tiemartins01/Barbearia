using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTOEnviarEmailValidator : AbstractValidator<DTOEnviarEmail>
{
    public DTOEnviarEmailValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254);
    }
}
