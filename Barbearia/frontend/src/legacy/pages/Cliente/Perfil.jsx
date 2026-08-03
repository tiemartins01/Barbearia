import { useEffect, useState } from "react";
import {
    ChevronRight,
    ChevronDown,
    User,
    CreditCard,
    LogOut,
} from "lucide-react";
import api from "../../services/api";
import { useNavigate } from "react-router-dom";

export default function Perfil() {
    const [abrirDados, setAbrirDados] = useState(false);
    const [abrirPagamento, setAbrirPagamento] = useState(false);
    const [dados, setDados] = useState({});
    const [loading, setLoading] = useState(false);
    const [sucesso, setSucesso] = useState(false);
    const navigate = useNavigate();

    async function Salvar() {

        setLoading(true);

        try {
            
            var response = await api.post("cliente/alterarDados", {
                nome: dados.nome,
                email: dados.email,
                telefone: dados.telefone,
                senhaAntiga: dados.senhaAntiga,
                novaSenha: dados.novaSenha,
                cpf: dados.cpf
            });
            setSucesso(true);
        } catch (error) {
            console.log("ERROR FULL:", error);
            alert(
                error.response?.data?.mensagem ??
                "Erro ao realizar a mudança dos dados."
            );
        } finally {
            setLoading(false);
        }

    }

    async function Sair() {
        setLoading(true);

        try {
            var resposta = await api.post("/login/logout");

            navigate("/", { replace: true });
        } catch (error) {
            console.error(error);

            alert(
                error.resposta?.data?.mensagem ??
                "Erro ao realizar logout."
            );
        } finally {
            setLoading(false);
        }
    }


    useEffect(() => {
        async function carregarDados() {
            try {
                const { data } = await api.get("cliente/dados");

                setDados({
                    nome: data.nome,
                    email: data.email,
                    telefone: data.telefone,
                    cpf: data.cpf,
                    senhaAntiga: "",
                    novaSenha: "",
                    qtdcortes: data.qtdcortes,
                    iniciais: data.iniciais
                });
            } catch (error) {
                console.log(error);
            }
        }

        carregarDados();
    }, []);

    function alterar(e) {
        setDados({
            ...dados,
            [e.target.name]: e.target.value,
        });
    }

    return (
        <div className=" mx-auto items-justify bg-background text-white px-4 py-6">

            {/* Perfil */}

            <div className="rounded-3xl bg-zinc-900 border border-zinc-800 p-8 text-center">

                <div className="mx-auto w-24 h-24 rounded-full bg-yellow-500 flex items-center justify-center text-4xl font-semibold text-black">
                    {dados.iniciais}
                </div>

                <h2 className="mt-5 text-3xl font-semibold">
                    {dados.nome}
                </h2>

                <p className="text-zinc-400 mt-2">
                    {dados.email?.emailPessoa}
                </p>

            </div>

            {/* Cortes */}

            <div className="mt-6 flex justify-center">

                <div className="w-[700px] h-[95px] rounded-3xl border border-zinc-800 bg-zinc-900 p-2 text-center">

                    <h1 className="text-5xl text-yellow-500 font-bold">
                        {dados.qtdcortes}
                    </h1>

                    <p className="mt-2 text-zinc-400">
                        Serviços concluidos
                    </p>

                </div>

            </div>

            {/* Menu */}

            <div className="mt-8 rounded-3xl border border-zinc-800 bg-zinc-900 overflow-hidden">

                {/* Meus dados */}

                <button
                    onClick={() => setAbrirDados(!abrirDados)}
                    className="w-full px-6 py-5 flex items-center justify-between hover:bg-zinc-800 transition"
                >

                    <div className="flex items-center gap-4">

                        <User className="text-yellow-500" />

                        <span className="text-lg">
                            Meus dados
                        </span>

                    </div>

                    {abrirDados ? (
                        <ChevronDown />
                    ) : (
                        <ChevronRight />
                    )}

                </button>

                {abrirDados && (

                    <div className="px-6 pb-6 border-t border-zinc-800 animate-in slide-in-from-top duration-300">

                        <div className="mt-6 space-y-4">

                            <input
                                name="nome"
                                value={dados.nome}
                                onChange={alterar}
                                placeholder="Nome"
                                className="w-full rounded-xl bg-zinc-800 p-3 outline-none"
                            />

                            <input
                                name="email"
                                value={dados.email}
                                onChange={alterar}
                                placeholder="E-mail"
                                className="w-full rounded-xl bg-zinc-800 p-3 outline-none"
                            />

                            <input
                                name="telefone"
                                value={dados.telefone || ""}
                                onChange={alterar}
                                placeholder="Telefone"
                                className="w-full rounded-xl bg-zinc-800 p-3 outline-none"
                            />

                            <input
                                name="cpf"
                                value={dados.cpf || ""}
                                onChange={alterar}
                                placeholder="CPF"
                                className="w-full rounded-xl bg-zinc-800 p-3 outline-none"
                            />

                            <input
                                type="password"
                                name="senhaAntiga"
                                value={dados.senhaAntiga || ""}
                                onChange={alterar}
                                placeholder="Senha antiga"
                                className="w-full rounded-xl bg-zinc-800 p-3 outline-none"
                            />

                            <input
                                type="password"
                                name="novaSenha"
                                value={dados.novaSenha || ""}
                                onChange={alterar}
                                placeholder="Nova senha"
                                className="w-full rounded-xl bg-zinc-800 p-3 outline-none"
                            />

                            <button
                                onClick={Salvar}
                                className="w-full rounded-xl bg-yellow-500 py-3 font-semibold text-black"
                            >
                                Salvar alterações
                            </button>

                        </div>

                    </div>

                )}

                {/* Pagamento */}

                <button
                    onClick={() => setAbrirPagamento(!abrirPagamento)}
                    className="w-full px-6 py-5 flex items-center justify-between hover:bg-zinc-800 transition"
                >
                    <div className="flex items-center gap-4">

                        <CreditCard className="text-yellow-500" />

                        <span className="text-lg">
                            Métodos de pagamento
                        </span>

                    </div>

                    {abrirPagamento ? (
                        <ChevronDown />
                    ) : (
                        <ChevronRight />
                    )}

                </button>

                {abrirPagamento && (

                    <div className="px-6 pb-6 border-t border-zinc-800 animate-in slide-in-from-top duration-300">

                        <div className="mt-6 space-y-4">
                            <div className="flex items-center justify-between rounded-xl bg-zinc-800 p-4">
                                <span className="text-lg">
                                    💳 Cartão de Crédito
                                </span>

                                <span className="text-green-400 font-medium">
                                    Aceito
                                </span>
                            </div>

                            <div className="flex items-center justify-between rounded-xl bg-zinc-800 p-4">
                                <span className="text-lg">
                                    💳 Cartão de Débito
                                </span>

                                <span className="text-green-400 font-medium">
                                    Aceito
                                </span>
                            </div>

                            <div className="flex items-center justify-between rounded-xl bg-zinc-800 p-4">
                                <span className="text-lg">
                                    📱 Pix
                                </span>

                                <span className="text-green-400 font-medium">
                                    Aceito
                                </span>
                            </div>

                            <div className="flex items-center justify-between rounded-xl bg-zinc-800 p-4">
                                <span className="text-lg">
                                    💵 Dinheiro
                                </span>

                                <span className="text-green-400 font-medium">
                                    Aceito
                                </span>
                            </div>
                        </div>

                    </div>

                )}

                {/* Sair */}

                <button
                    className="w-full px-6 py-5 flex items-center justify-between border-t border-zinc-800 hover:bg-zinc-800" onClick={Sair}
                >
                    
                    <div className="flex items-center gap-4">

                        <LogOut className="text-yellow-500" />

                        <span className="text-lg">
                            Sair
                        </span>

                    </div>

                    <ChevronRight />

                </button>

            </div>

        </div>
    );
}