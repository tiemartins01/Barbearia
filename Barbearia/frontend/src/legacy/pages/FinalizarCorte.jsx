import { useState, useEffect } from "react";
import api from "../services/api";
import { getUserId, getUserRole } from "./Login";

export default function FinalizarCorte() {

    const [idBarbeiro, setIdBarbeiro] = useState("");
    const [dataHora, setDataHora] = useState("");
    const [pagamento, setPagamento] = useState("");
    const [nomes, setNomes] = useState([]);

    const role = Number(getUserRole());
    console.log("Role atual:", role);

    useEffect(() => {

        if (role == 1) {
            carregarBarbeiros();
        }

    }, [role]);

    async function carregarBarbeiros() {

        try {

            const response =
                await api.get("/api/login/nomesbarbeiros");

            setNomes(response.data || []);

        } catch {

            setNomes([]);

        }

    }

    async function handleFinalizar(e) {

        e.preventDefault();

        if (!dataHora || !pagamento) {
            alert("Preencha os campos");
            return;
        }

        let id = null;

        if (role == 2) {

            id = getUserId();

        }
        else if (role == 1) {

            if (!idBarbeiro) {
                alert("Selecione o barbeiro");
                return;
            }

            id = idBarbeiro;

        }

        try {

            await api.post(
                "/api/horarioCliente/concluir",
                {
                    idBarber: Number(id),
                    dataHora: dataHora,
                    formaPagamento: Number(pagamento)
                }
            );

            alert("Pagamento realizado!");

        }
        catch (error) {

            console.log(error.response?.data);

            alert("Erro ao finalizar corte");

        }

    }

    return (
        <div className="container">
            <div className="card">

                <h1>Finalizar Corte</h1>

                <form onSubmit={handleFinalizar}>

                    {role == 1 && (
                        <select
                            value={idBarbeiro}
                            onChange={(e) =>
                                setIdBarbeiro(e.target.value)
                            }
                        >
                            <option value="">
                                Selecionar barbeiro
                            </option>

                            {nomes.map((b) => (
                                <option
                                    key={b.id_barbeiro}
                                    value={b.id_barbeiro}
                                >
                                    {b.nome}
                                </option>
                            ))}

                        </select>
                    )}

                    <input
                        type="datetime-local"
                        value={dataHora}
                        onChange={(e) =>
                            setDataHora(e.target.value)
                        }
                    />

                    <select
                        value={pagamento}
                        onChange={(e) =>
                            setPagamento(e.target.value)
                        }
                    >
                        <option value="">
                            Forma de pagamento
                        </option>

                        <option value="1">
                            Dinheiro
                        </option>

                        <option value="2">
                            Cartão
                        </option>

                        <option value="3">
                            Pix
                        </option>

                    </select>

                    <button type="submit">
                        Finalizar Corte
                    </button>

                </form>

            </div>
        </div>
    );
}