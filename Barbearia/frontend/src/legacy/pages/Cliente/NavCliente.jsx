import { useNavigate } from "react-router-dom"
import { Home, Scissors, Calendar, History, User } from "lucide-react";
export default function NavCliente() {

    const navigate = useNavigate();
    return (
        <nav className="mb-2 fixed inset-x-0 bottom-4 mx-auto flex max-w-md items-center justify-between rounded-2xl border bg-white px-4 py-3">
            <button onClick={() => navigate("/cliente")}>
                <Home />
            </button>

            <button onClick={() => navigate("/servicostotal")}>
                <Scissors />
            </button>

            <button onClick={() => navigate("/cliente/marcar")}>
                <Calendar />
            </button>

            <button onClick={() => navigate("/cliente/historico")}>
                <History />
            </button>

            <button onClick={() => navigate("/cliente/dados")}>
                <User />
            </button>
        </nav>
    );
}