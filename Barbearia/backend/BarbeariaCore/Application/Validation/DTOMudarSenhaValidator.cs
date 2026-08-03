using Barbearia.Core.DTO;
using FluentValidation;

namespace Barbearia.Core.Application.Validation;

public sealed class DTOMudarSenhaValidator : AbstractValidator<DTOMudarSenha>
{
    public DTOMudarSenhaValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254);
        RuleFor(x => x.Codigo).NotEmpty().Length(6);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6).MaximumLength(128);
        RuleFor(x => x.SenhaRepetida)
            .Equal(x => x.Senha).WithMessage("As senhas precisam ser iguais.");
    }
}
