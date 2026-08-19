using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTOLoginUsuarioValidator : AbstractValidator<DTOLoginUsuario>
{
    public DTOLoginUsuarioValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Login é obrigatório.")
            .MaximumLength(50).WithMessage("Login deve ter no máximo 50 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MaximumLength(128).WithMessage("Senha deve ter no máximo 128 caracteres.");
    }
}
