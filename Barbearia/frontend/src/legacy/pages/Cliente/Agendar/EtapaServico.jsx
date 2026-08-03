import { useEffect, useState } from "react";
import { ChevronRight, Scissors } from "lucide-react";
import api from "../../../services/api";

export default function EtapaServico({ selecionarServico }) {

    const [servicos, setServicos] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {

        async function carregarServicos() {

            try {

                const response = await api.get("/servicos/ativos");

                setServicos(response.data);

            } catch (error) {

                console.error(error);

            } finally {

                setLoading(false);

            }

        }

        carregarServicos();

    }, []);

    if (loading) {

        return (

            <div className="py-20 text-center text-muted-foreground">
                Carregando serviços...
            </div>

        );

    }

    return (

        <>

            <section>

                <h2 className="font-serif text-3xl">
                    Escolha um serviço
                </h2>

                <p className="text-sm text-muted-foreground mt-2">
                    Selecione o serviço desejado para continuar.
                </p>

            </section>

            <section className="mt-8 space-y-4">

                {servicos.map((servico) => (

                    <button
                        key={servico.id}
                        onClick={() => selecionarServico(servico)}
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
                                    h-14
                                    w-14
                                    rounded-2xl
                                    bg-muted
                                    flex
                                    items-center
                                    justify-center
                                "
                            >

                                <Scissors className="h-6 w-6 text-primary" />

                            </div>

                            <div className="text-left">

                                <h3 className="font-semibold text-lg">

                                    {servico.nomeServico}

                                </h3>

                                <p className="text-sm text-muted-foreground">

                                    {servico.duracao} minutos

                                </p>

                            </div>

                        </div>

                        <div className="flex items-center gap-4">

                            <span className="font-semibold text-primary text-lg">

                                R$ {Number(servico.preco).toFixed(2)}

                            </span>

                            <ChevronRight
                                className="h-5 w-5 text-muted-foreground"
                            />

                        </div>

                    </button>

                ))}

            </section>

        </>

    );

}