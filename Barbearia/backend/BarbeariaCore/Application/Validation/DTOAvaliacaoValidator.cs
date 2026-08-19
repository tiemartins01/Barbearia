using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTOAvaliacaoValidator : AbstractValidator<DTOAvaliacao>
{
    public DTOAvaliacaoValidator()
    {
        RuleFor(x => x.Id_barbeiro).GreaterThan(0);
        RuleFor(x => x.Id_horario).GreaterThan(0);
        RuleFor(x => x.Id_servico).GreaterThan(0);
        RuleFor(x => x.Nota).InclusiveBetween(1, 5);
        RuleFor(x => x.Comentario).MaximumLength(128);
    }
}
