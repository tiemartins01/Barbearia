using BarbeariaCore.Application.DTOs;
using FluentValidation;

namespace BarbeariaCore.Application.Validation;

public sealed class DTOMarcarHorarioValidator : AbstractValidator<DTOMarcarHorario>
{
    public DTOMarcarHorarioValidator()
    {
        RuleFor(x => x.Id_barbeiro).GreaterThan(0);
        RuleFor(x => x.Id_servico).GreaterThan(0);
        RuleFor(x => x.horario).NotEmpty();
    }
}
