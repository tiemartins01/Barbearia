import { useEffect, useState } from "react";
import api from "../../services/api";
import { Star, X } from "lucide-react";

export default function HistoricoServicos() {
    const [servicos, setServicos] = useState([]);
    const [modalAberto, setModalAberto] = useState(false);
    const [servicoSelecionado, setServicoSelecionado] = useState(null);
    const [nota, setNota] = useState(0);
    const [notaHover, setNotaHover] = useState(0);
    const [comentario, setComentario] = useState("");
    const [enviando, setEnviando] = useState(false);
    const [infoHorario, setInfoHorario] = useState(null);

    const [pagina, setPagina] = useState(1);
    const [temMais, setTemMais] = useState(true);

    const pageSize = 3;

    useEffect(() => {
        carregarMais(1);
    }, []);

    async function carregarMais(paginaAtual = pagina) {
        try {

            const response = await api.get("/cliente/historico", {
                params: {
                    page: paginaAtual,
                    pageSize
                }
            });

            const novosServicos = response.data;;

            setServicos(prev =>
                paginaAtual === 1
                    ? novosServicos
                    : [...prev, ...novosServicos]
            );

            if (novosServicos.length < pageSize) {
                setTemMais(false);
            } else {
                setPagina(paginaAtual + 1);
            }

        } catch (err) {
            console.error(err);
        }
    }

    async function abrirModal(servico) {
        try {
            const response = await api.post("/cliente/infoHorario", {
                idHorario: servico.id,
            });

            setInfoHorario(response.data);
            setServicoSelecionado(servico);
            setNota(0);
            setNotaHover(0);
            setComentario("");
            setModalAberto(true);
        } catch (err) {
            console.error(err);
        }
    }

    function fecharModal() {
        setModalAberto(false);
        setServicoSelecionado(null);
        setInfoHorario(null);
        setNota(0);
        setNotaHover(0);
        setComentario("");
    }

    async function enviarAvaliacao() {
        if (nota === 0 || !infoHorario) return;

        setEnviando(true);

        try {
            await api.post("/cliente/avaliacao", {
                id_barbeiro: infoHorario.id_barbeiro,
                id_horario: infoHorario.id,
                id_servico: infoHorario.id_servico,
                nota,
                comentario,
                horario: infoHorario.horario,
            });

            setServicos(prev =>
                prev.map(item =>
                    item.id === servicoSelecionado.id
                        ? { ...item, podeAvaliar: false }
                        : item
                )
            );

            fecharModal();
        } catch (err) {
            console.error(err);
        } finally {
            setEnviando(false);
        }
    }

    return (
        <div className="min-h-screen bg-background text-foreground px-3 py-6">
            <div className="space-y-5">
                {servicos.map((servico) => {
                    const data = new Date(servico.data).toLocaleDateString(
                        "pt-BR",
                        {
                            day: "2-digit",
                            month: "short",
                            year: "numeric",
                        }
                    );

                    return (
                        <div
                            key={servico.id}
                            className="rounded-3xl border border-zinc-800 bg-zinc-900 p-5"
                        >
                            <div className="flex justify-between items-start">
                                <div>
                                    <h2 className="text-zinc-400 text-xl font-semibold">
                                        {servico.nomeServico}
                                    </h2>

                                    <p className="text-zinc-400">
                                        com {servico.nomeBarbeiro}
                                    </p>
                                </div>

                                <span className="text-xl font-semibold text-yellow-500">
                                    R$ {Number(servico.valorServico).toFixed(2)}
                                </span>
                            </div>

                            <hr className="my-5 border-zinc-800" />

                            <div className="flex justify-between items-center">
                                <span className="text-zinc-400">{data}</span>

                                <button
                                    onClick={() => abrirModal(servico)}
                                    disabled={!servico.podeAvaliar}
                                    className={`flex items-center gap-1 font-medium ${servico.podeAvaliar
                                            ? "text-yellow-500 cursor-pointer"
                                            : "text-zinc-500 cursor-not-allowed"
                                        }`}
                                >
                                    Avaliar
                                    <Star size={15} fill="currentColor" />
                                </button>
                            </div>
                        </div>
                    );
                })}
            </div>

            {temMais && (
                <button
                    onClick={() => carregarMais(pagina)}
                    className="mt-6 w-full rounded-xl border border-zinc-700 py-3 hover:bg-zinc-800 transition"
                >
                    Carregar mais
                </button>
            )}

            {modalAberto && servicoSelecionado && (
                <div
                    className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 px-4"
                    onClick={fecharModal}
                >
                    <div
                        className="w-full max-w-sm rounded-3xl border border-zinc-800 bg-zinc-900 p-6"
                        onClick={(e) => e.stopPropagation()}
                    >
                        <div className="flex justify-between items-start mb-4">
                            <div>
                                <h3 className="text-lg font-semibold text-zinc-100">
                                    Avaliar serviço
                                </h3>

                                <p className="text-zinc-400 text-sm">
                                    {servicoSelecionado.nomeServico} com{" "}
                                    {servicoSelecionado.nomeBarbeiro}
                                </p>
                            </div>

                            <button
                                onClick={fecharModal}
                                className="text-zinc-400 hover:text-white"
                            >
                                <X size={20} />
                            </button>
                        </div>

                        <div className="flex justify-center gap-2 mb-5">
                            {[1, 2, 3, 4, 5].map((valor) => (
                                <button
                                    key={valor}
                                    type="button"
                                    onMouseEnter={() => setNotaHover(valor)}
                                    onMouseLeave={() => setNotaHover(0)}
                                    onClick={() => setNota(valor)}
                                >
                                    <Star
                                        size={32}
                                        className="text-yellow-500"
                                        fill={
                                            (notaHover || nota) >= valor
                                                ? "currentColor"
                                                : "none"
                                        }
                                    />
                                </button>
                            ))}
                        </div>

                        <textarea
                            value={comentario}
                            onChange={(e) => setComentario(e.target.value)}
                            placeholder="Escreva um comentário (opcional)"
                            rows={4}
                            className="w-full resize-none rounded-2xl border border-zinc-800 bg-zinc-950 p-3 text-zinc-100 placeholder-zinc-500 focus:outline-none focus:ring-1 focus:ring-yellow-500"
                        />

                        <button
                            onClick={enviarAvaliacao}
                            disabled={nota === 0 || enviando || !infoHorario}
                            className="mt-4 w-full rounded-2xl bg-yellow-500 py-3 font-semibold text-zinc-950 disabled:opacity-40"
                        >
                            {enviando ? "Enviando..." : "Enviar avaliação"}
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}