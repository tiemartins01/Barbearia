import { useState, useEffect } from "react"; // guarda valores na tela
import { useNavigate, Link } from "react-router-dom";
import api from "../services/api";

export default function Login() {
    // valores digitados
    const [login, setLogin] = useState("");
    const [senha, setSenha] = useState("");
    const [erro, SetErro] = useState("");
    const [loading, setLoading] = useState(false);

    // estado da verificação de sessão já existente
    const [carregando, setCarregando] = useState(true);

    const navigate = useNavigate(); // permite navegar entre as páginas ex: /menu

    // Roda UMA VEZ quando o componente monta, verifica se já existe sessão válida
    useEffect(() => {
        async function verificarAutenticacao() {
            try {
                const me = await api.get("/login/me");
                const rotas = {
                    Admin: "/admin",
                    Barbeiro: "/barbeiro",
                    Cliente: "/cliente"
                };

                const rota = rotas[me.data.role];
                if (rota) {
                    navigate(rota); // já tem cookie válido, pula o login
                    return;
                }
            } catch (error) {
                // sem sessão válida (401/erro) -> comportamento normal, fica na tela de login
                navigate("/")
            } finally {
                setCarregando(false);
            }
        }

        verificarAutenticacao();
    }, [navigate]);

    async function handleLogin(e) { // quando clica em entrar, chama a função
        e.preventDefault(); // impede de recarregar a página
        const loginFormatado = login.trim();
        const senhaFormatada = senha; // sem trim: espaço pode ser parte intencional da senha
        SetErro("");

        if (!loginFormatado) {
            SetErro("Informe seu login!");
            return;
        }

        if (!senhaFormatada) {
            SetErro("Informe sua senha!");
            return;
        }

        try {
            setLoading(true);

            await api.post("/login", {
                nome: loginFormatado,
                senha: senhaFormatada
            });

            const me = await api.get("/login/me");

            const rotas = {
                Admin: "/admin",
                Barbeiro: "/barbeiro",
                Cliente: "/cliente"
            };

            const rota = rotas[me.data.role];
            if (!rota) {
                SetErro("Perfil inválido.");
                return;
            }
            navigate(rota);
        } catch (error) {
            SetErro(
                error.response?.data?.mensagem ??
                "Não foi possível realizar o login."
            );
            setTimeout(() => {
                SetErro("");
            }, 3000);
        } finally {
            setLoading(false);
        }
    }

    // Enquanto verifica se já existe sessão, não mostra nada (evita "piscar" a tela de login)
    if (carregando) {
        return (
            <div className="w-screen h-screen bg-gray-100 flex items-center justify-center">
                <p className="text-gray-500">Carregando...</p>
            </div>
        );
    }

    return (
        <div className="w-screen h-screen bg-gray-100 flex items-center justify-center">
            <div className=" w-[400px] h-[380px] border-4 border-black rounded-2xl shadow-2xl bg-white">

                <h1 className="text-4xl font-bold text-center m-8 tracking-wide">Login</h1>

                <form onSubmit={handleLogin} className="flex flex-col items-center gap-5">

                    <input
                        type="text"
                        placeholder="Login"
                        autoComplete="username"
                        value={login}
                        disabled={loading}
                        onChange={(e) => setLogin(e.target.value)}
                        className="border-2 border-gray-400 text-lg outline-none focus: border-black text-center"
                    />

                    <input
                        type="password"
                        placeholder="Senha"
                        autoComplete="current-password"
                        value={senha}
                        disabled={loading}
                        onChange={(e) => setSenha(e.target.value)}
                        className="border-2 border-gray-400 text-lg outline-none focus: border-black text-center"
                    />

                    <button type="submit" disabled={loading} className="text-xl mt-4 bg-gray-300 text-black rounded-xl font-semibold transition-all duration-200 hover:scale-105 hover:bg-green-500 active:scale-95 cursor-pointer px-5">
                        {loading ? "Entrando..." : "Entrar"}
                    </button>

                    <Link to="/novo" onClick={(e) => { if (loading) e.preventDefault(); }} className={`text-blue-400 underline ${loading ? "pointer-events-none opacity-50 cursor-not-allowed" : "hover:text-blue-800"}`}>
                        Não tem conta? Criar conta
                    </Link>

                    <Link to="/esqueci-senha" onClick={(e) => { if (loading) e.preventDefault(); }} className={`text-blue-400 underline ${loading ? "pointer-events-none opacity-50 cursor-not-allowed" : "hover:text-blue-800"}`}>
                        Esqueci a senha
                    </Link>

                    {erro && (
                        <div className="bg-red-100 border border-red-400 text-red-700 px-7 py-1 rounded-xl text-center mt-16 ">
                            {erro}
                        </div>
                    )}
                </form>
            </div>
        </div>
    );
}