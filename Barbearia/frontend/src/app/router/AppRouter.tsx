import { Navigate, Route, Routes } from 'react-router-dom';
import { LoginPage } from '../../pages/login/LoginPage';
import { HomePage } from '../../pages/home/HomePage';
import { ProtectedRoute } from '../../features/auth/ui/ProtectedRoute';

import Cadastro from '../../legacy/pages/Cadastro';
import EsqueciSenha from '../../legacy/pages/EsqueciP';
import MudarSenha from '../../legacy/pages/MudarSenha';
import Admin from '../../legacy/pages/OpcoesA';
import Barbeiro from '../../legacy/pages/OpcoesB';
import Cliente from '../../legacy/pages/Cliente/Cliente';
import ServicosCliente from '../../legacy/pages/ServicosCliente';
import Agendamento from '../../legacy/pages/Cliente/Agendamento';
import Historico from '../../legacy/pages/Cliente/Historico';
import DadosCliente from '../../legacy/pages/Cliente/Perfil-Cliente';

export function AppRouter() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/novo" element={<Cadastro />} />
      <Route path="/esqueci-senha" element={<EsqueciSenha />} />
      <Route path="/trocar" element={<MudarSenha />} />

      <Route element={<ProtectedRoute />}>
        <Route path="/" element={<HomePage />} />
      </Route>

      <Route element={<ProtectedRoute roles={['Admin']} />}>
        <Route path="/admin" element={<Admin />} />
      </Route>

      <Route element={<ProtectedRoute roles={['Barbeiro']} />}>
        <Route path="/barbeiro" element={<Barbeiro />} />
      </Route>

      <Route element={<ProtectedRoute roles={['Cliente']} />}>
        <Route path="/cliente" element={<Cliente />} />
        <Route path="/servicostotal" element={<ServicosCliente />} />
        <Route path="/cliente/marcar" element={<Agendamento />} />
        <Route path="/cliente/historico" element={<Historico />} />
        <Route path="/cliente/dados" element={<DadosCliente />} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
