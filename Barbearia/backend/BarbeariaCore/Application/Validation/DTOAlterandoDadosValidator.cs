using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTOAlterandoDadosValidator : AbstractValidator<DTOAlterandoDados>
{
    public DTOAlterandoDadosValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254);
        RuleFor(x => x.Telefone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Cpf).NotEmpty().MaximumLength(14);
        RuleFor(x => x.SenhaAntiga).NotEmpty().MaximumLength(128);
        RuleFor(x => x.NovaSenha).NotEmpty().MinimumLength(6).MaximumLength(128);
    }
}
