import { useState } from "react"; // guarda valores na tela
import { useNavigate } from "react-router-dom";
import api from "../services/api";

export default function RecuperarSenhaUser() {

    const [email, setEmail] = useState("");
    const [loading, setLoading] = useState(false);
    const [erro, SetErro] = useState("");
    const [ok, SetOK] = useState("");

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

    function validaEmail() {
        if (!email.trim())
        {
            mostrarErro("Campo e-mail vazio!");
            return false;
        }
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailRegex.test(email)) {
            mostrarErro("E-mail inválido.");
            return false;
        }
        return true; 
    }


    const navigate = useNavigate();

    async function handleRecuperar(e) { // quando clica em entrar, chama a função
        e.preventDefault(); // imprede de recarregar a página

        if (!validaEmail())
            return;


        try {
            setLoading(true);
            await api.post("/envioe", {
                email: email.trim().toLowerCase(),
            });

            mostrarSucesso("Caso o e-mail exista, enviaremos as instruções.")

            setTimeout(() => {
                navigate("/trocar");
            }, 2000);

            
        } catch (error) {

            const mensagem = error?.response?.data?.mensagem || "Não foi possível processar sua solicitação.";
            mostrarErro(mensagem);
        }
        finally {
            setLoading(false);
        }
    }


    function handleCancelar() {
        navigate("/");
    }

    return (
        <div className="w-screen h-screen bg-gray-100 flex items-center justify-center">
            <div className="w-[420px] h-[320px] border-4 border-black rounded-2xl shadow-2xl bg-white">

                <h1 className="text-4xl font-bold text-center m-8 tracking-wide">Recuperar Senha</h1>

                <form onSubmit={handleRecuperar}className="flex flex-col items-center gap-5">
                   
                    <input
                        type="email"
                        placeholder="E-mail"
                        autoComplete="email"
                        value={email}
                        disabled={loading}
                        onChange={(e) => setEmail(e.target.value)}
                        className="text-center border-2 border-gray-400 text-lg outline-none focus:border-black text-black"
                    />

                    <div className="flex">

                        <button type="submit"
                            disabled={loading} className="text-xl mt-4 rounded-xl text-black font-semibold transition-all duration:200 hover:scale-105 hover:bg-green-500 active:scale-95 cursor-pointer px-5">{
                                loading ? "Enviando" : "Enviar"
                            }                      
                        </button>

                        <button
                            type="button"
                            onClick={handleCancelar}
                            disabled={loading}
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


