import { useState, useEffect } from "react";
import api from "../services/api";
import { getUserId } from "../pages/Login" // COM ESSA FUNÇÃO QUE CONSIGO PEGAR O ID DE QUEM ESTÁ ACESSANDO E CARREGAR OS HORÁRIOS DO BARBEIRO QUE ACESSOU



export default function HorariosFuturos() {
    const [horarios, setHorarios] = useState([]);
    const [loading, setLoading] = useState(false);

    async function carregarHorarios() {
        try {
            setLoading(true);
            const id = getUserId();
            const response = await api.post("/api/futuros", {
                Id_barbeiro: Number(id)
            });

            setHorarios(response.data || []);
        } catch (error) {
            if (error.response?.status === 204) {
                setHorarios([]);
                return;
            }
        } finally {
            setLoading(false);
        }
    }
    // ao abrir, já carrega, [] significa carregar apenas uma vez
    useEffect(() => {
        carregarHorarios();
    }, []);

    return (
        <div className="hf-wrapper">
            <div className="hf-card">

                <div className="hf-header">
                    <h2>📅 Horários Futuros</h2>
                    <button onClick={carregarHorarios}>Atualizar</button>
                </div>

                {loading ? (
                    <p className="hf-status">Carregando...</p>
                ) : horarios.length === 0 ? (
                    <div className="hf-empty">
                        <p>Nenhum horário encontrado</p>
                    </div>
                ) : (
                    <table className="hf-table">
                        <thead>
                            <tr>
                                <th>Cliente</th>
                                <th>Serviço</th>
                                <th>Data</th>
                                <th>Hora</th>
                            </tr>
                        </thead>

                        <tbody>
                            {horarios.map((h, index) => {
                                const data = new Date(h.horario);

                                return (
                                    <tr key={index}>
                                        <td>{h.nome_cliente}</td>
                                        <td>{h.nome_servico}</td>
                                        <td>{data.toLocaleDateString("pt-BR")}</td>
                                        <td>
                                            {data.toLocaleTimeString("pt-BR", {
                                                hour: "2-digit",
                                                minute: "2-digit",
                                            })}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
     )
}