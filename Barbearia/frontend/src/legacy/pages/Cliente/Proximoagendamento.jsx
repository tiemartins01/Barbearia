import { Calendar, Clock } from "lucide-react";

export default function AgendamentoCliente({ proximoAgendamento }) {

    const horario = proximoAgendamento
        ? new Date(proximoAgendamento.horario)
        : null;

    const data = horario?.toLocaleDateString("pt-BR");

    const hora = horario?.toLocaleTimeString("pt-BR", {
        hour: "2-digit",
        minute: "2-digit",
    });

    return (
        <section className="mt-6">
            <article className="rounded-3xl border border-primary/40 bg-card p-5">

                <p className="text-[11px] font-medium uppercase tracking-[0.25em] text-primary">
                    Próximo agendamento
                </p>

                {proximoAgendamento ? (
                    <>
                        <h2 className="mt-2 font-serif text-2xl">
                            {proximoAgendamento.nomeServico}
                        </h2>

                        <p className="text-sm text-muted-foreground">
                            com {proximoAgendamento.nomeBarbeiro}
                        </p>

                        <div className="mt-5 flex gap-6">

                            <div className="flex items-center gap-2 text-sm">
                                <Calendar className="h-4 w-4 text-primary" />
                                <span>{data}</span>
                            </div>

                            <div className="flex items-center gap-2 text-sm">
                                <Clock className="h-4 w-4 text-primary" />
                                <span>{hora}</span>
                            </div>

                        </div>
                    </>
                ) : (
                    <>
                        <h2 className="mt-2 font-serif text-2xl">
                            Nenhum agendamento
                        </h2>

                        <p className="mt-1 text-sm text-muted-foreground">
                            Você ainda não possui horários marcados.
                        </p>
                    </>
                )}

            </article>
        </section>
    );
}