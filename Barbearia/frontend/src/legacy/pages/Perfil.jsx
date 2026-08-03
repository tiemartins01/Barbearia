import { useEffect, useState } from "react";
import api from "../services/api";

export default function Perfil() {

    const [perfil, setPerfil] = useState(null);
    const [loading, setLoading] = useState(false);

    const imagemPadrao = "/images/padrao.jpg";

    async function carregarPerfil() {
        try {
            setLoading(true);

            const response = await api.get("/api/perfil");
            console.log(response);
            setPerfil(response.data);

        } catch (error) {
            console.error(error);
            alert("Erro ao carregar perfil");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        carregarPerfil();
    }, []);

    return (
        <div className="perfil-wrapper">

            <div className="perfil-card">

                <h1>👤 Meu Perfil</h1>

                {loading ? (

                    <p>Carregando...</p>

                ) : perfil && (

                    <div className="perfil-container">

                        {/* IMAGEM */}

                        <div className="perfil-imagem">

                            <img
                                src={
                                    perfil.foto
                                        ? perfil.foto
                                        : imagemPadrao
                                }
                                alt="Foto do usuário"
                                onError={(e) => {
                                    e.currentTarget.src =
                                        imagemPadrao;
                                }}
                            />

                        </div>

                        {/* INFORMAÇÕES */}

                        <div className="perfil-info">

                            <p>
                                <strong>Nome:</strong>
                                {" "}
                                {perfil.nome}
                            </p>

                            <p>
                                <strong>Email:</strong>
                                {" "}
                                {perfil.email}
                            </p>

                            <p>
                                <strong>Telefone:</strong>
                                {" "}
                                {perfil.number}
                            </p>

                            <p>
                                <strong>CPF:</strong>
                                {" "}
                                {perfil.cpf}
                            </p>

                            <p>
                                <strong>Login:</strong>
                                {" "}
                                {perfil.login}
                            </p>

                        </div>

                    </div>

                )}

            </div>

        </div>
    );
}