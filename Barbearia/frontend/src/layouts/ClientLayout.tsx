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

  const initials =
    user?.nome
      ?.split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((name) => name[0])
      .join('')
      .toUpperCase() || 'U';

  return (
    <div className="client-shell">
      <header className="client-navbar">
        <div className="client-navbar-inner">
          <div className="client-brand">
            <span className="brand-scissors">✂</span>
            <span>Barber<strong>Shop</strong></span>
          </div>

          <nav className="client-nav" aria-label="Navegação do cliente">
            <NavLink end to="/cliente">Início</NavLink>
            <NavLink to="/cliente/agendar">Agendar</NavLink>
            <NavLink to="/cliente/servicos">Serviços</NavLink>
            <NavLink to="/cliente/historico">Histórico</NavLink>
            <NavLink to="/cliente/perfil">Perfil</NavLink>
          </nav>

          <div className="client-user">
            <div className="client-avatar">{initials}</div>
            <div className="client-user-text">
              <strong>{user?.nome}</strong>
              <span>{user?.role}</span>
            </div>
            <button className="client-logout" disabled={logout.isPending} onClick={signOut}>
              {logout.isPending ? 'Saindo...' : 'Sair'}
            </button>
          </div>
        </div>
      </header>

      <main className="client-content">
        <Outlet />
      </main>
    </div>
  );
}
