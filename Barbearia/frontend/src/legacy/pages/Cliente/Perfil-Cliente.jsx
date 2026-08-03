import { useState } from "react";
import HeaderCLiente from "../Cliente/Header-Cliente"
import NavCliente from "../Cliente/NavCliente"
import DadosCliente from "../Cliente/Perfil";
export default function Cliente() {


    const [usuario, setUsuario] = useState(null);
    const [dados, setDados] = useState(null);

    return (
        <div className="min-h-screen bg-background pb-28 text-foreground">
            <div className="mx-auto max-w-md px-5 pt-8">

                <HeaderCLiente usuario={usuario} />
                <DadosCliente dados={dados} />
                <NavCliente />
            </div>
        </div>
    );
}