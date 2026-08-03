import { useState } from "react";
import HeaderCLiente from "../pages/Cliente/Header-Cliente"
import NavCliente from "../pages/Cliente/NavCliente"
import ServicosCliente from "../pages/Cliente/Servicos"
export default function Servicos() {

    const [usuario, setUsuario] = useState(null);
    const [servicos, setServicos] = useState([]);
     
    return (
        <div className="min-h-screen bg-background text-foreground pb-28">

            <div className="mx-auto max-w-md px-5 pt-8">

                <HeaderCLiente usuario={usuario} />
                <ServicosCliente servicos={servicos} />
                <NavCliente/>
            </div>

        </div>
    );
}