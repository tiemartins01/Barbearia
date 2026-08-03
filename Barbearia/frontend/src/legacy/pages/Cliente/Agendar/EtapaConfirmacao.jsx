import { useState } from "react";
import api from "../../../services/api";
import {
    Calendar,
    Clock,
    Scissors,
    User,
    CheckCircle
} from "lucide-react";

export default function EtapaConfirmacao({
    agendamento,
    novoAgendamento
}) {

    const [loading, setLoading] = useState(false);
    const [sucesso, setSucesso] = useState(false);

    async function finalizarAgendamento() {

        setLoading(true);

        try {
            const response = await api.post("/agendamento/marcar", {

                id_barbeiro: agendamento.idBarbeiro,
                id_servico: agendamento.idServico,
                horario: agendamento.horario

            });

            setSucesso(true);

        } catch (error) {
            console.log("ERROR FULL:", error);

            alert(
                error.response?.data?.mensagem ??
                "Erro ao realizar agendamento."
            );
        } finally {

            setLoading(false);

        }

    }

    if (sucesso) {

        return (

            <div className="text-center py-16">

                <CheckCircle
                    size={90}
                    className="mx-auto text-green-500"
                />

                <h2 className="text-3xl font-serif mt-6">

                    Agendamento realizado!

                </h2>

                <p className="text-muted-foreground mt-2">

                    Seu horário foi reservado com sucesso.

                </p>

                <button

                    onClick={novoAgendamento}

                    className="
                        mt-10
                        w-full
                        rounded-2xl
                        bg-primary
                        py-4
                        font-semibold
                        text-primary-foreground
                    "

                >

                    Novo Agendamento

                </button>

            </div>

        );

    }

    return (

        <>

            <section>

                <h2 className="font-serif text-3xl">

                    Confirmar Agendamento

                </h2>

                <p className="text-muted-foreground mt-2">

                    Confira as informações antes de finalizar.

                </p>

            </section>

            <section className="mt-8 space-y-4">

                <div className="rounded-2xl border bg-card p-5 flex gap-4">

                    <Scissors className="text-primary" />

                    <div>

                        <p className="text-sm text-muted-foreground">

                            Serviço

                        </p>

                        <h3 className="font-semibold">

                            {agendamento.nomeServico}

                        </h3>

                    </div>

                </div>

                <div className="rounded-2xl border bg-card p-5 flex gap-4">

                    <User className="text-primary" />

                    <div>

                        <p className="text-sm text-muted-foreground">

                            Barbeiro

                        </p>

                        <h3 className="font-semibold">

                            {agendamento.nomeBarbeiro}

                        </h3>

                    </div>

                </div>

                <div className="rounded-2xl border bg-card p-5 flex gap-4">

                    <Calendar className="text-primary" />

                    <div>

                        <p className="text-sm text-muted-foreground">

                            Data

                        </p>

                        <h3 className="font-semibold">

                            {new Date(agendamento.horario)
                                .toLocaleDateString("pt-BR")}

                        </h3>

                    </div>

                </div>

                <div className="rounded-2xl border bg-card p-5 flex gap-4">

                    <Clock className="text-primary" />

                    <div>

                        <p className="text-sm text-muted-foreground">

                            Horário

                        </p>

                        <h3 className="font-semibold">

                            {new Date(agendamento.horario)
                                .toLocaleTimeString("pt-BR", {

                                    hour: "2-digit",
                                    minute: "2-digit"

                                })}

                        </h3>

                    </div>

                </div>

                <div className="rounded-2xl border bg-card p-5">

                    <div className="flex justify-between">

                        <span className="text-muted-foreground">

                            Valor

                        </span>

                        <span className="font-bold text-primary">

                            R$ {Number(agendamento.preco).toFixed(2)}

                        </span>

                    </div>

                </div>

            </section>

            <button

                onClick={finalizarAgendamento}

                disabled={loading}

                className="
                    w-full
                    mt-8
                    rounded-2xl
                    bg-primary
                    py-4
                    font-semibold
                    text-primary-foreground
                    disabled:opacity-50
                    hover:scale-110
                    hover:bg-green-500
                "

            >

                {loading
                    ? "Agendando..."
                    : "Confirmar Agendamento"}

            </button>

        </>

    );

}