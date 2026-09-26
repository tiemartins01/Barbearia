import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../features/auth/model/authStore';
import { useLogoutMutation } from '../features/auth/hooks/useAuth';

export function ClientLayout() {
  const user = useAuthStore((s) => s.user);
  const logout = useLogoutMutation();
  const navigate = useNavigate();

  async function signOut() {
    await logout.mutateAsync().catch(() => undefined);
    navigate('/login', { replace: true });
  }

  return (
    <div className="client-shell">
      <aside className="client-sidebar">
        <div className="brand">BarberShop</div>
        <nav aria-label="Navegação do cliente">
          <NavLink end to="/cliente">Início</NavLink>
          <NavLink to="/cliente/agendar">Agendar</NavLink>
          <NavLink to="/cliente/servicos">Serviços</NavLink>
          <NavLink to="/cliente/historico">Histórico</NavLink>
          <NavLink to="/cliente/perfil">Perfil</NavLink>
        </nav>

        <div className="sidebar-user">
          <strong>{user?.nome}</strong>
          <span>{user?.role}</span>
          <button className="button secondary" disabled={logout.isPending} onClick={signOut}>
            {logout.isPending ? 'Saindo...' : 'Sair'}
          </button>
        </div>
      </aside>

      <main className="client-content">
        <Outlet />
      </main>
    </div>
  );
}
