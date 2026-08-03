import { useAuthStore } from '../../features/auth/model/authStore';
export function HomePage(){const user=useAuthStore(s=>s.user);return <main><h1>Olá, {user?.nome}</h1><p>Fundação da área autenticada.</p></main>}
