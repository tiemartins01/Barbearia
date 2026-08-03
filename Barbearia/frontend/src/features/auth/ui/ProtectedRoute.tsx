import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore, type UserRole } from '../model/authStore';

type ProtectedRouteProps = {
  roles?: UserRole[];
};

export function ProtectedRoute({ roles }: ProtectedRouteProps) {
  const location = useLocation();
  const user = useAuthStore((state) => state.user);
  const initialized = useAuthStore((state) => state.initialized);

  if (!initialized) {
    return <main className="page-state">Carregando sessão...</main>;
  }

  if (!user) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  if (roles && !roles.includes(user.role)) {
    return <Navigate to={user.role === 'Cliente' ? '/cliente' : '/'} replace />;
  }

  return <Outlet />;
}
