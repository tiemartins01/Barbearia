import { useState } from "react"; // guarda valores na tela
import { useNavigate } from "react-router-dom";
import api from "../services/api";

export default function AlterarSenhaUser() {

    const [email, setEmail] = useState("");
    const [password, SetPassword] = useState("");
    const [repeatpassword, SetRepeat] = useState("");
    const [codigo, Setcodigo] = useState("");
    const [erro, SetErro] = useState("");
    const [ok, SetOK] = useState("");
    const [loading, setLoading] = useState(false);

    function mostrarErro(mensagem) {
        SetErro(mensagem);

        setTimeout(() => {
            SetErro("");
        }, 3000);
    }

    function mostrarSucesso(mensagem) {
        SetOK(mensagem);

        setTimeout(() => {
            SetOK("");
        }, 3000);
    }

    function validaDados() {
        if (!email.trim() || !password.trim() || !repeatpassword.trim() || !codigo.trim()) {
            mostrarErro("Preencha todos os dados!");
            return false;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

        if (!emailRegex.test(email)) {

            mostrarErro("E-mail inválido.");

            return false;
        }
        if (password !== repeatpassword) {

            mostrarErro("As senhas são diferentes.");

            return false;
        }

        return true;
    }

    const navigate = useNavigate();

    async function handleAlterar(e) { // quando clica em entrar, chama a função
        e.preventDefault(); // imprede de recarregar a página

        if (!validaDados())
            return;

        try {
            setLoading(true);

            await api.post("/trocar", {
                email: email.trim(),
                senha: password.trim(),
                senhaRepetida: repeatpassword.trim(),
                codigo: codigo.trim()

            });

            mostrarSucesso("Senha alterada com sucesso!");
            setTimeout(() => {
                navigate("/");
            }, 3000);

            setEmail("");
            SetPassword("");
            SetRepeat("");
            Setcodigo("");

        } catch (error) {

            const mensagem =
                error.response?.data?.mensagem || "Erro ao tentar alterar a senha!";

            mostrarErro(mensagem);
        } finally {
            setLoading(false);
        }
    }
    function handleCancelar() {
        navigate("/");
    }

    return (
        <div className="w-screen h-screen bg-gray-100 flex items-center justify-center">
            <div className="w-[430px] h-[520px] border-4 border-black rounded-2xl shadow-2xl bg-white">

                <h1 className="text-4xl font-bold text-center m-8 tracking-wide">Alterar Senha</h1>
                <form onSubmit={handleAlterar} className="flex flex-col items-center gap-5">
                   
                    <input
                        type="email"
                        placeholder="E-mail"
                        autoComplete="email"
                        value={email}
                        disabled={loading}
                        onChange={(e) => setEmail(e.target.value)}
                        className="text-center border-2 border-gray-400 text-lg outline-none focus:border-black text-black"
                    />

                    <input
                        type="password"
                        placeholder="Senha"
                        autoComplete="new-password"
                        value={password}
                        disabled={loading}
                        onChange={(e) => SetPassword(e.target.value)}
                        className="text-center border-2 border-gray-400 text-lg outline-none focus:border-black text-black"
                    />

                    <input
                        type="password"
                        placeholder="Repita a senha"
                        autoComplete="new-password"
                        value={Repeatpassword}
                        disabled={loading}
                        onChange={(e) => SetRepeat(e.target.value)}
                        className="text-center border-2 border-gray-400 text-lg outline-none focus:border-black text-black"
                    />

                    <input
                        type="text"
                        placeholder="Código"
                        maxLength={6}
                        value={codigo}
                        disabled={loading}
                        onChange={(e) => Setcodigo(e.target.value)}
                        className="text-center border-2 border-gray-400 text-lg outline-none focus:border-black text-black"
                    />


                    <div className="flex">

                        <button disabled={loading} type="submit" className="text-xl mt-4 rounded-xl text-black font-semibold transition-all duration:200 hover:scale-105 hover:bg-green-500 active:scale-95 cursor-pointer px-5">
                            {loading ? "Alterando" : "Alterar"}
                        </button>

                        <button
                            type="button"
                            disabled={loading}
                            onClick={handleCancelar}
                            className="text-xl mt-4 rounded-xl text-black font-semibold transition-all duration:200 hover:scale-105 hover:bg-red-500 active:scale-95 cursor-pointer px-5"
                        >
                            Cancelar
                        </button>

                    </div>
                    {erro && (
                        <div className="bg-red-100 border border-red-400 text-red-700 px-7 py-1 rounded-xl text-center mt-2 ">
                            {erro}
                        </div>
                    )}
                    {ok && (
                        <div className="bg-green-100 border border-green-400 text-green-700 px-7 py-1 rounded-xl text-center">
                            {ok}
                        </div>
                    )} 
                </form>
            </div>
        </div>
    );
}


