import { useEffect, useState } from "react";
import api from "../../services/api";
import HeaderCLiente from "../Cliente/Header-Cliente"
import AgendamentoCliente from "../Cliente/Agendar/Agendamento"
import NavCliente from "../Cliente/NavCliente"
export default function Cliente() {


    const [usuario, setUsuario] = useState(null);
    const [barbeiros, SetBarbeiros] = useState([]);


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


    return (
        <div className="min-h-screen bg-background pb-28 text-foreground">
            <div className="mx-auto max-w-md px-5 pt-8">

                <HeaderCLiente usuario={usuario} />
                <AgendamentoCliente barbeiros={barbeiros} />
                <NavCliente />
            </div>
        </div>
    );
}