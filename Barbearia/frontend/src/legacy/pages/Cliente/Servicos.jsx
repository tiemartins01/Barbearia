import { useEffect, useState } from "react";
import api from "../../services/api";
import { Scissors, ChevronRight } from "lucide-react";

export default function ServicosCliente() {

    const [servicos, setServicos] = useState([]);

    useEffect(() => {

        async function carregarServicos() {
            try {

                const response = await api.get("/servicos/ativos");
                setServicos(response.data);

            } catch (error) {
                console.error(error);
            }
        }
        carregarServicos();

    }, []);

    return (
        <div className="min-h-screen bg-background text-foreground pb-28">

            <div className="mx-auto max-w-md px-5 pt-8">

                <section className="mt-8">

                    <h2 className="font-serif text-3xl">
                        Serviços
                    </h2>

                    <p className="text-muted-foreground text-sm mt-1">
                        Escolha o serviço que deseja agendar
                    </p>

                </section>

                <section className="mt-6 space-y-4">

                    {servicos.map((servico) => (

                        <button
                            key={servico.id}
                            //onClick={() => selecionarServico(servico)}
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
                                cursor-not-allowed  
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

                                    <h3 className="font-medium text-lg">
                                        {servico.nomeServico}
                                    </h3>

                                    <p className="text-sm text-muted-foreground">
                                        {servico.duracao} min
                                    </p>

                                </div>

                            </div>

                            <div className="flex items-center gap-4">

                                <span className="text-xl font-semibold text-primary">
                                    R$ {servico.preco}
                                </span>
                            </div>
                        </button>

                    ))}

                </section>

            </div>
        </div>
    );
}