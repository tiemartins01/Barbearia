import { useEffect, useState } from "react";
import { ChevronRight, Star, User } from "lucide-react";
import api from "../../../services/api";

export default function EtapaBarbeiro({ selecionarBarbeiro }) {

    const [barbeiros, setBarbeiros] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {

        async function carregarBarbeiros() {

            try {

                const response = await api.get("cliente/barbeiros");

                setBarbeiros(response.data);

            } catch (error) {

                console.error(error);

            } finally {

                setLoading(false);

            }

        }

        carregarBarbeiros();

    }, []);

    if (loading) {

        return (

            <div className="py-20 text-center text-muted-foreground">
                Carregando barbeiros...
            </div>

        );

    }

    return (

        <>

            <section>

                <h2 className="font-serif text-3xl">
                    Escolha um barbeiro
                </h2>

                <p className="text-sm text-muted-foreground mt-2">
                    Selecione quem realizará seu atendimento.
                </p>

            </section>

            <section className="mt-8 space-y-4">

                {barbeiros.map((barbeiro) => (

                    <button
                        key={barbeiro.id}
                        onClick={() => selecionarBarbeiro(barbeiro)}
                        className="
                            w-full
                            rounded-3xl
                            border
                            bg-card
                            p-4
                            flex
                            items-center
                            justify-between
                            transition
                            hover:border-primary
                            hover:shadow-lg 
                            hover:scale-110
                        "
                    >

                        <div className="flex items-center gap-4">

                            <div
                                className="
                                    h-16
                                    w-16
                                    rounded-full
                                    bg-muted
                                    flex
                                    items-center
                                    justify-center
                                "
                            >

                                <User className="h-8 w-8 text-primary" />

                            </div>

                            <div className="text-left">

                                <h3 className="font-semibold text-lg">
                                    {barbeiro.nome}
                                </h3>

                                <div className="flex items-center gap-2 mt-1">

                                    <Star
                                        className="text-yellow-500"
                                        size={16}
                                        fill="currentColor"
                                    />

                                    <span className="text-sm text-muted-foreground">

                                        {barbeiro.nota
                                            ? barbeiro.nota.toFixed(1)
                                            : "Novo barbeiro"}

                                    </span>

                                </div>

                            </div>

                        </div>

                        <ChevronRight
                            className="text-muted-foreground"
                            size={22}
                        />

                    </button>

                ))}

            </section>

        </>

    );

}