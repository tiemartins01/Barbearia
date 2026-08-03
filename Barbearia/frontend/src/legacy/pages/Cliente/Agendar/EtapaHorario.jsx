import { useEffect, useState } from "react";
import api from "../../../services/api";
import { CalendarDays, Clock } from "lucide-react";

export default function EtapaHorario({
    agendamento,
    confirmarHorario
}) {

    const hoje = new Date().toISOString().split("T")[0];

    const [data, setData] = useState(hoje);
    const [horarios, setHorarios] = useState([]);
    const [horarioSelecionado, setHorarioSelecionado] = useState("");
    const [loading, setLoading] = useState(false);

    useEffect(() => {

        async function carregarHorarios() {

            if (!agendamento.idBarbeiro) return;

            setLoading(true);
            setHorarioSelecionado("");

            try {

                const response = await api.get("/agendamento/horarioslivres", {
                    params: {
                        id_barbeiro: agendamento.idBarbeiro,
                        data: data
                    }
                });

                setHorarios(response.data || []);

            } catch (error) {

                console.log(error);
                setHorarios([]);

            } finally {

                setLoading(false);

            }
        }

        carregarHorarios();

    }, [data, agendamento.idBarbeiro]);

    function selecionarHorario(hora) {
        setHorarioSelecionado(hora);
    }

    function horarioPassado(hora) {

        // Apenas verifica horários da data atual
        if (data !== hoje) return false;

        const agora = new Date();

        const [h, m] = hora.split(":").map(Number);

        const horario = new Date();
        horario.setHours(h, m, 0, 0);

        return horario <= agora;
    }

    function continuar() {

        if (!horarioSelecionado) return;

        confirmarHorario(`${data}T${horarioSelecionado}`);
    }

    return (
        <>
            <section>
                <h2 className="font-serif text-3xl">
                    Escolha um horário
                </h2>

                <p className="mt-2 text-sm text-muted-foreground">
                    Selecione uma data e um horário disponível.
                </p>
            </section>

            {/* DATA */}
            <section className="mt-8">

                <label className="text-sm font-medium">
                    Data
                </label>

                <div className="relative mt-2">

                    <CalendarDays
                        size={20}
                        className="absolute left-4 top-3 text-muted-foreground"
                    />

                    <input
                        type="date"
                        value={data}
                        min={hoje}
                        onChange={(e) => setData(e.target.value)}
                        className="w-full rounded-2xl border bg-card py-3 pl-12 pr-4 outline-none"
                    />

                </div>

            </section>

            {/* HORÁRIOS */}
            <section className="mt-8">

                <h3 className="mb-4 font-medium">
                    Horários disponíveis
                </h3>

                {loading ? (

                    <div className="rounded-2xl border bg-card py-8 text-center text-muted-foreground">
                        Carregando horários...
                    </div>

                ) : horarios.length === 0 ? (

                    <div className="rounded-2xl border border-dashed py-8 text-center text-muted-foreground">
                        Nenhum horário disponível.
                    </div>

                ) : (

                    <div className="grid grid-cols-3 gap-3">

                        {horarios.map((hora) => {

                            const passado = horarioPassado(hora);

                            return (

                                <button
                                    key={hora}
                                    type="button"
                                    disabled={passado}
                                    onClick={() => selecionarHorario(hora)}
                                    className={`
                                        flex items-center justify-center gap-2
                                        rounded-xl border py-3
                                        transition-all duration-200

                                        ${passado
                                            ? "border-primary bg-primary text-primary-foreground shadow-lg opacity-50 cursor-not-allowed"
                                            : horarioSelecionado === hora
                                            ? "border-primary bg-amber-300 text-primary-foreground shadow-lg scale-110 active:scale-95"
                                                : "bg-card hover:border-primary hover:bg-primary/10 active:scale-95"
                                        }
                                    `}
                                >

                                    <Clock size={16} />

                                    <span className="font-medium">
                                        {hora}
                                    </span>

                                </button>

                            );

                        })}

                    </div>

                )}

            </section>

            {/* BOTÃO */}
            <button
                type="button"
                disabled={!horarioSelecionado}
                onClick={continuar}
                className="
                    mt-10 w-full rounded-2xl
                    bg-primary py-4
                    font-semibold text-primary-foreground
                    transition
                    disabled:cursor-not-allowed
                    disabled:opacity-50
                    hover:scale-110
                    hover:bg-green-500
                    
                "
            >
                Continuar
            </button>
        </>
    );
}