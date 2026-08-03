import { useEffect, useState } from "react";
import api from "../../services/api";
import HeaderCLiente from "../Cliente/Header-Cliente"
import ProximoAgendamentoCliente from "../Cliente/Proximoagendamento"
import BarbeiroCliente from "../Cliente/Barbeiros-Cliente"
import UnidadeCliente from "../Cliente/Unidade-Cliente"
import NavCliente from "../Cliente/NavCliente"
import { useLocation } from "react-router-dom";
export default function Cliente() {

    const location = useLocation();
    const [usuario, setUsuario] = useState(null);
    const [barbeiros, SetBarbeiros] = useState([]);
    const [proximoAgendamento, setProximoAgendamento] = useState(null);


    useEffect(() => {
        async function carregarBarbeiro() {
            try {
                const lista = await api.get("cliente/barbeiros");
                SetBarbeiros(lista.data);
            } catch (error) {
                console.log(error);
            }
        } carregarBarbeiro();
    }, []);

    useEffect(() => {
        async function carregarProximoAtendimento() {
            try {
                const proximo = await api.get("agendamento/proximo");
                setProximoAgendamento(proximo.data);
                console.log(proximo.data);
            } catch (error) {
                console.log(error);
            }
        } carregarProximoAtendimento();
    }, [location.key]);


    return (
        <div className="min-h-screen bg-background pb-28 text-foreground">
            <div className="mx-auto max-w-md px-5 pt-8">

                <HeaderCLiente usuario={usuario} />
                <ProximoAgendamentoCliente proximoAgendamento={proximoAgendamento} />
                <BarbeiroCliente barbeiros={barbeiros} />
                <UnidadeCliente />
                <NavCliente />
            </div>
        </div>
    );
}