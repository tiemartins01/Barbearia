import { useEffect, useState } from "react";
import api from "../../services/api";
import HeaderCLiente from "../Cliente/Header-Cliente"
import NavCliente from "../Cliente/NavCliente"
import HistoricoServico from "../Cliente/HistoricoServicos"
export default function Historico() {


    const [usuario, setUsuario] = useState(null);
    const [historico, setHistorico] = useState([]);


    return (
        <div className="min-h-screen bg-background pb-28 text-foreground">
            <div className="mx-auto max-w-md px-5 pt-8">

                <HeaderCLiente usuario={usuario} />
                <div >
                    <h1 className="m-4 font-serif text-3xl"> Histórico </h1>
                    <h3 className="m-4 font-serif text-2xl"> Seus serviços anteriores </h3>
                </div>
                
                <HistoricoServico historico={historico}/>
                <NavCliente/>
            </div>
        </div>
    );
}