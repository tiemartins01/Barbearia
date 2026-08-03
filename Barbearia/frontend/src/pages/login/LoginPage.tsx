import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { getCurrentUser, login } from '../../features/auth/api/authApi';
import { useAuthStore } from '../../features/auth/model/authStore';

const schema = z.object({
  login: z.string().trim().min(1, 'Informe o login.'),
  senha: z.string().min(6, 'A senha deve ter pelo menos 6 caracteres.'),
});

type FormData = z.infer<typeof schema>;

type LocationState = { from?: { pathname?: string } };

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const setUser = useAuthStore((state) => state.setUser);
  const setInitialized = useAuthStore((state) => state.setInitialized);
  const { register, handleSubmit, formState: { errors, isSubmitting }, setError } = useForm<FormData>({ resolver: zodResolver(schema) });

  async function submit(data: FormData) {
    try {
      await login(data.login, data.senha);
      const user = await getCurrentUser();
      setUser(user);
      setInitialized(true);

      const requestedPath = (location.state as LocationState | null)?.from?.pathname;
      const rolePath = user.role === 'Cliente' ? '/cliente' : user.role === 'Barbeiro' ? '/barbeiro' : '/admin';
      navigate(requestedPath ?? rolePath, { replace: true });
    } catch {
      setError('root', { message: 'Login ou senha inválidos.' });
    }
  }

  return (
    <main className="auth">
      <form onSubmit={handleSubmit(submit)}>
        <h1>BarberShop</h1>
        <label>Login<input autoComplete="username" {...register('login')} /></label>
        <span>{errors.login?.message}</span>
        <label>Senha<input type="password" autoComplete="current-password" {...register('senha')} /></label>
        <span>{errors.senha?.message}</span>
        <span>{errors.root?.message}</span>
        <button disabled={isSubmitting}>{isSubmitting ? 'Entrando...' : 'Entrar'}</button>
        <div className="auth-links"><Link to="/novo">Criar conta</Link><Link to="/esqueci-senha">Esqueci minha senha</Link></div>
      </form>
    </main>
  );
}
