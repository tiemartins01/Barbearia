import { useNavigate } from "react-router-dom";

export default function OpcoesBarbeiro() {

    const navigate = useNavigate();

    return (
        <div className="container">
            <div className="card">

                <h1>Opções do Barbeiro</h1>

                <div className="buttons-opcoes-admin">

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
                        onClick={() => navigate("/perfil")}
                    >
                        ALTERAR PERFIL BARBEIRO
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/perfil")}
                    >
                        RELATÓRIO DIÁRIO
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/perfil")}
                    >
                        RELATÓRIO MENSAL
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/perfil")}
                    >
                        CADASTRAR SERVIÇO
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/perfil")}
                    >
                        ALTERAR SERVIÇO
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/perfil")}
                    >
                       CANCELAR HORÁRIO
                    </button>

                    <button
                        type="button"
                        onClick={() => navigate("/perfil")}
                    >
                        AGENDA DO BARBEIRO
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