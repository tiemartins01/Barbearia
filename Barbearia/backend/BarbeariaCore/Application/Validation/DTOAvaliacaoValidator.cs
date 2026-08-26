using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTOAvaliacaoValidator : AbstractValidator<DTOAvaliacao>
{
    public DTOAvaliacaoValidator()
    {
        RuleFor(x => x.Nota).InclusiveBetween(1, 5);
        RuleFor(x => x.Comentario).MaximumLength(128);
    }
}
