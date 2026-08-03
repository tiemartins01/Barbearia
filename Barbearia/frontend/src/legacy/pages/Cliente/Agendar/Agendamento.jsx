import { useState } from "react";

import BarraProgresso from "../Agendar/BarraProgresso";
import EtapaServico from "../Agendar/EtapaServico";
import EtapaBarbeiro from "../Agendar/EtapaBarbeiro";
import EtapaHorario from "../Agendar/EtapaHorario";
import EtapaConfirmacao from "../Agendar/EtapaConfirmacao";

export default function Agendar() {

    const [etapa, setEtapa] = useState(1);

    const [agendamento, setAgendamento] = useState({

        idServico: null,
        nomeServico: "",
        preco: 0,
        duracao: 0,

        idBarbeiro: null,
        nomeBarbeiro: "",

        horario: null

    });

    function selecionarServico(servico) {

        setAgendamento(prev => ({

            ...prev,

            idServico: servico.id,
            nomeServico: servico.nomeServico,
            preco: servico.preco,
            duracao: servico.duracao

        }));

        setEtapa(2);
    }

    function selecionarBarbeiro(barbeiro) {

        setAgendamento(prev => ({

            ...prev,

            idBarbeiro: barbeiro.id,
            nomeBarbeiro: barbeiro.nome

        }));

        setEtapa(3);

    }

    function confirmarHorario(horario) {

        setAgendamento(prev => ({

            ...prev,

            horario

        }));

        setEtapa(4);

    }

    function novoAgendamento() {

        setAgendamento({

            idServico: null,
            nomeServico: "",
            preco: 0,
            duracao: 0,

            idBarbeiro: null,
            nomeBarbeiro: "",

            horario: null

        });

        setEtapa(1);

    }

    return (

        <div className="min-h-screen bg-background text-foreground pb-28">

            <div className="mx-auto max-w-md px-5 pt-8">

               <h2 className="font-serif text-3xl">
                        Agendar
               </h2>

                <div className="mt-6">
                    <BarraProgresso etapa={etapa} />
                </div>

                <div className="mt-8">

                    {etapa === 1 && (

                        <EtapaServico
                            selecionarServico={selecionarServico}
                        />

                    )}

                    {etapa === 2 && (

                        <EtapaBarbeiro
                            selecionarBarbeiro={selecionarBarbeiro}
                        />

                    )}

                    {etapa === 3 && (

                        <EtapaHorario
                            agendamento={agendamento}
                            confirmarHorario={confirmarHorario}
                        />

                    )}

                    {etapa === 4 && (

                        <EtapaConfirmacao
                            agendamento={agendamento}
                            novoAgendamento={novoAgendamento}
                        />

                    )}

                </div>

            </div>

        </div>

    );

}