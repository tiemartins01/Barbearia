using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTOMarcarHorarioValidator : AbstractValidator<DTOMarcarHorario>
{
    public DTOMarcarHorarioValidator()
    {
        RuleFor(x => x.BarbeiroId).GreaterThan(0);
        RuleFor(x => x.ServicoId).GreaterThan(0);
        RuleFor(x => x.Horario).NotEmpty();
    }
}
