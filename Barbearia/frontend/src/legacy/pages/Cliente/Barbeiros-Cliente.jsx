import {Star } from "lucide-react";
import { useEffect, useState } from "react";
import api from "../../services/api";
import Rating from "./Rating";
export default function BarbeiroCliente() {

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
        <section className="mt-6">
            <h3 className="font-serif text-xl mb-3">
                Nossos Barbeiros
            </h3>

            <div className="flex gap-3 overflow-x-auto">
                {barbeiros.map((b) => (
                    <article
                        key={b.iniciais}
                        className="min-w-[180px] rounded-2xl border p-4"
                    >
                        <div className="grid h-14 w-14 place-items-center rounded-full bg-black text-white">
                            {b.iniciais}
                        </div>

                        <div className="mt-3 text-sm font-medium">
                            {b.nome}
                        </div>

                        <div className="text-xs text-gray-500">
                            {b.especialidade}
                        </div>

                        <div className="mt-2 flex items-center gap-2 text-xs">
                            <Rating value={b.notaMedia} size={14} />
                            <span>
                                {b.notaMedia.toFixed(1)} ({b.quantidadeAvaliacoes})
                            </span>
                        </div>
                    </article>
                ))}
            </div>
        </section>
    );
}