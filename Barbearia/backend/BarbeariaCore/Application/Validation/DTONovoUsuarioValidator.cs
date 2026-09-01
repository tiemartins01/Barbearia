using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTONovoUsuarioValidator : AbstractValidator<DTONovoUsuario>
{
    public DTONovoUsuarioValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254);
        RuleFor(x => x.Telefone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Cpf).NotEmpty().MaximumLength(14);
        RuleFor(x => x.Login).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(6).MaximumLength(128);
        RuleFor(x => x.Foto).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Foto));
    }
}
