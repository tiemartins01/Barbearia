import { useState } from "react"; // guarda valores na tela
import { useNavigate } from "react-router-dom";
import api from "../services/api";

export default function CadastroUser() {

    
    const [nome, setNome] = useState("");
    const [email, setEmail] = useState("");
    const [phone, setPhone] = useState("");
    const [cpf, setCpf] = useState("");
    const [login, setLogin] = useState("");
    const [senha, setSenha] = useState("");
    const [erro, SetErro] = useState("");
    const [ok, SetOK] = useState("");
    const [loading, setLoading] = useState(false);
    

    const navigate = useNavigate();
    const nomeFormatado = nome.trim();
    const emailFormatado = email.trim();
    const phoneFormatado = phone.trim();
    const cpfFormatado = cpf.trim();
    const loginFormatado = login.trim();
    const senhaFormatado = senha.trim();
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

    function formatarCPF(value) {
        let numbers = value.replace(/\D/g, "");
        numbers = numbers.slice(0, 11);
        numbers = numbers.replace(/^(\d{3})(\d)/, "$1.$2");
        numbers = numbers.replace(/^(\d{3})\.(\d{3})(\d)/, "$1.$2.$3");
        numbers = numbers.replace(/\.(\d{3})(\d)/, ".$1-$2");

        return numbers;
    }

    function formatPhone(value) {
        let numbers = value.replace(/\D/g, "");
        numbers = numbers.slice(0, 11);
        numbers = numbers.replace(/^(\d{2})(\d)/, "($1) $2");
        numbers = numbers.replace(/(\d{5})(\d)/, "$1-$2");
        return numbers;
    }

    function validarCliente() {
        if (!nomeFormatado || emailFormatado || !phoneFormatado || !cpfFormatado || !loginFormatado || !senhaFormatado) {
            MostrarErro("Preencha todos os campos!");
            return false;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailRegex.test(email)) {
            mostrarErro("E-mail inválido!");
            return false;
        }

        const cpfNumero = cpf.replace(/\D/g, "");

        if (cpfNumero.length != 11) {
            mostrarErro("CPF inválido!");
            return false;
        }

        const totalNumeros = phone.replace(/\D/g, "");

        if (totalNumeros.length != 11) {
            mostrarErro("Telefone inválido!");
            return false;
        }

        return true;

    }

    async function handleCadastro(e) { // quando clica em entrar, chama a função
        e.preventDefault(); // imprede de recarregar a página

        if (!validarCliente())
            return;

        try {
            setLoading(true);

            await api.post("/cadastro", {
                nome: nome,
                email: email,
                phone: phone,
                cpf: cpf,
                login: login,
                senhaR: senha,
            });
            mostrarSucesso("Usuário cadastrado com sucesso!");

            setTimeout(() => {
                navigate("/");
            },1000);

        } catch (error) {
                const mensagem =
                error.response?.data?.mensagem ||
                "Erro ao tentar cadastrar!";

            mostrarErro(mensagem);

            return;
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
            <div className="w-[420px] h-[600px] border-4 border-black rounded-2xl shadow-2xl bg-white">

                <h1 className="text-4xl font-bold text-center m-8">Cadastro</h1>

                <form onSubmit={handleCadastro} className="flex flex-col items-center gap-5">

                    <input
                        type="text"
                        placeholder="Nome"
                        autoComplete="name"
                        value={nome}
                        disabled={loading}
                        onChange={(e) => setNome(e.target.value)}
                        className = "border-2 border-gray-400 text-lg outline-none focus: border-black text-center"
                    />

                    <input
                        type="email"
                        placeholder="E-mail"
                        autoComplete="email"
                        value={email}
                        disabled={loading}
                        onChange={(e) => setEmail(e.target.value)}
                        className="border-2 border-gray-400 text-lg outline-none focus: border-black text-center"
                    />

                    <input
                        type="text"
                        placeholder="Telefone"
                        value={phone}
                        autoComplete="tel"
                        disabled={loading}
                        onChange={(e) => setPhone(formatPhone(e.target.value))}
                        className="border-2 border-gray-400 text-lg outline-none focus: border-black text-center"
                    />

                    <input
                        type="text"
                        placeholder="CPF"
                        value={cpf}
                        disabled={loading}
                        onChange={(e) => setCpf(formatarCPF(e.target.value))}
                        className="border-2 border-gray-400 text-lg outline-none focus: border-black text-center"
                    />

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
                        autoComplete="new-password"
                        value={senha}
                        disabled={loading}
                        onChange={(e) => setSenha(e.target.value)}
                        className="border-2 border-gray-400 text-lg outline-none focus: border-black text-center"
                    />

                    <div className="flex gap-5">

                        <button disabled={loading} className="text-xl bg-green-300 text-white px-4 py-2 rounded-xl hover:bg-green-700" type="submit">
                            {loading ? "Cadastrando..." : "Cadastrar"}
                        </button>

                        <button
                            className="text-xl bg-red-300 text-white px-4 py-2 rounded-xl hover:bg-red-700" type="button"
                            onClick={handleCancelar}
                            
                        >
                            Cancelar
                        </button>
                    </div>
                    {erro && (
                        <div className="bg-red-100 border border-red-400 text-red-700 px-7 py-1 rounded-xl text-center mt-16 ">
                            {erro}
                        </div>
                    )}
                    {ok && (
                        <div className="bg-green-100 border border-green-400 text-green-700 px-7 py-1 rounded-xl text-center mt-2">
                            {ok}
                        </div>
                    )} 
                </form>
            </div>
        </div>
    );
}


