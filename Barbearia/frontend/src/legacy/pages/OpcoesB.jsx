import { useNavigate } from "react-router-dom";

export default function OpcoesBarbeiro() {

    const navigate = useNavigate();

    return (
        <div className="container">
            <div className="card">

                <h1>Opções do Barbeiro</h1>

                <div className="buttons-opcoes-barbeiro">

                    <button
                        type="button"
                        onClick={() => navigate("/futuros")}
                    >
                        HORÁRIOS AGENDADOS
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/perfil")}
                    >
                        MEU PERFIL
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/finalizarcorte")}
                    >
                        FINALIZAR CORTE
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/")}
                        className="bt_cancel"
                    >
                        SAIR
                    </button>

                </div>

            </div>
        </div>
    );
}