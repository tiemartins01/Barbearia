using BarbeariaCore.Domain.Exceptions;

namespace BarbeariaCore.Domain.Policies
{
    public static class PoliticaAgenda
    {
        public static readonly TimeOnly HorarioAbertura =
            new(8, 0);

        public static readonly TimeOnly HorarioFechamento =
            new(18, 0);

        public const int IntervaloMinutos = 30;

        public static void ValidarHorarioFuturo(
            DateTime horario,
            DateTime agora)
        {
            if (horario <= agora)
            {
                throw new DomainException(
                    "APPOINTMENT_DATE_INVALID",
                    "O horário do agendamento deve estar no futuro.");
            }
        }

        public static void ValidarDataNaoPassada(
            DateOnly data,
            DateOnly hoje)
        {
            if (data < hoje)
            {
                throw new DomainException(
                    "APPOINTMENT_DATE_INVALID",
                    "A data informada não pode estar no passado.");
            }
        }

        public static void ValidarHorarioNaGrade(
            DateTime horario)
        {
            var hora =
                TimeOnly.FromDateTime(horario);

            if (hora < HorarioAbertura ||
                hora >= HorarioFechamento)
            {
                throw new DomainException(
                    "APPOINTMENT_OUTSIDE_BUSINESS_HOURS",
                    "O horário está fora do funcionamento da barbearia.");
            }

            var minutosDesdeAbertura =
                (hora.Hour * 60 + hora.Minute)
                -
                (HorarioAbertura.Hour * 60
                 + HorarioAbertura.Minute);

            if (minutosDesdeAbertura %
                IntervaloMinutos != 0)
            {
                throw new DomainException(
                    "APPOINTMENT_INVALID_TIME_SLOT",
                    "O horário não pertence à grade de agendamentos.");
            }
        }

        public static void ValidarDuracao(
            int duracaoMinutos)
        {
            if (duracaoMinutos <= 0)
            {
                throw new DomainException(
                    "APPOINTMENT_INVALID_DURATION",
                    "A duração deve ser maior que zero.");
            }

            if (duracaoMinutos %
                IntervaloMinutos != 0)
            {
                throw new DomainException(
                    "APPOINTMENT_INVALID_DURATION",
                    "A duração deve respeitar os intervalos da agenda.");
            }
        }

        public static void
            ValidarTerminoDentroDoExpediente(
                DateTime inicio,
                int duracaoMinutos)
        {
            ValidarDuracao(duracaoMinutos);

            if (!CabeNoExpediente(
                    inicio,
                    duracaoMinutos))
            {
                throw new DomainException(
                    "APPOINTMENT_EXCEEDS_BUSINESS_HOURS",
                    "O atendimento ultrapassa o horário de funcionamento.");
            }
        }

        public static bool CabeNoExpediente(
            DateTime inicio,
            int duracaoMinutos)
        {
            if (duracaoMinutos <= 0)
                return false;

            var fim =
                inicio.AddMinutes(duracaoMinutos);

            var horarioFim =
                TimeOnly.FromDateTime(fim);

            return horarioFim <=
                   HorarioFechamento;
        }

        public static bool ExisteSobreposicao(
            DateTime inicioNovo,
            DateTime fimNovo,
            DateTime inicioExistente,
            DateTime fimExistente)
        {
            return inicioNovo < fimExistente &&
                   fimNovo > inicioExistente;
        }

        public static void GarantirDisponibilidade(
            bool existeConflito)
        {
            if (existeConflito)
            {
                throw new DomainException(
                    "APPOINTMENT_TIME_CONFLICT",
                    "O barbeiro já possui um agendamento ativo neste período.");
            }
        }

        public static IReadOnlyCollection<TimeOnly>
            GerarGradeHorario()
        {
            var horarios =
                new List<TimeOnly>();

            var atual = HorarioAbertura;

            while (atual < HorarioFechamento)
            {
                horarios.Add(atual);

                atual = atual.AddMinutes(
                    IntervaloMinutos);
            }

            return horarios;
        }

        public static IReadOnlyCollection<DateTime>
            GerarSlotsOcupados(
                DateTime inicio,
                int duracaoMinutos)
        {
            ValidarDuracao(
                duracaoMinutos);

            var quantidadeSlots =
                duracaoMinutos /
                IntervaloMinutos;

            var slots =
                new List<DateTime>();

            for (var i = 0;
                 i < quantidadeSlots;
                 i++)
            {
                slots.Add(
                    inicio.AddMinutes(
                        i * IntervaloMinutos));
            }

            return slots;
        }
    }
}