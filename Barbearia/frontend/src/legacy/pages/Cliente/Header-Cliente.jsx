import { useEffect, useState } from "react";
import api from "../../services/api";

export default function HeaderCliente() {

    const [usuario, setUsuario] = useState(null);


    useEffect(() => {
        async function carregarUsuario() {
            try {
                const response = await api.get("/login/me");
                setUsuario(response.data);

            } catch (err) {
                console.error(err);
            }
        }

        carregarUsuario();
    }, []);

    //👋
    return (
        <div className="bg-background text-foreground">
            <div className="px-5 pt-8">

                <header className="flex items-start justify-between">
                    <div>
                        <p className="text-[11px] uppercase tracking-[0.25em] text-muted-foreground">
                            Bem-vindo
                        </p>
                        <h1 className="mt-1 font-serif text-3xl">
                            Olá, {usuario?.nome} 
                        </h1>
                    </div>

                </header>
            </div>
        </div>
    );
}