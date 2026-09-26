import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { useLoginMutation } from '../../features/auth/hooks/useAuth';
import { normalizeApiError } from '../../shared/api/apiError';

const schema = z.object({
  login: z.string().trim().min(1, 'Informe o login.'),
  senha: z.string().min(6, 'A senha deve ter pelo menos 6 caracteres.'),
});

type FormData = z.infer<typeof schema>;
type LocationState = { from?: { pathname?: string }; accountCreated?: boolean };

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const loginMutation = useLoginMutation();
  const state = location.state as LocationState | null;

  const {
    register,
    handleSubmit,
    formState: { errors },
    setError,
  } = useForm<FormData>({ resolver: zodResolver(schema) });

  async function submit(data: FormData) {
    try {
      const user = await loginMutation.mutateAsync({ nome: data.login, senha: data.senha });
      const wanted = state?.from?.pathname;
      const fallback =
        user.role === 'Cliente' ? '/cliente' :
        user.role === 'Barbeiro' ? '/barbeiro' :
        '/admin';

      navigate(wanted ?? fallback, { replace: true });
    } catch (error) {
      const api = normalizeApiError(error);
      const message =
        api.status === 401 ? 'Login ou senha inválidos.' :
        api.status === 429 ? 'Muitas tentativas. Aguarde e tente novamente.' :
        api.code === 'NETWORK_ERROR' ? 'Servidor indisponível. Tente novamente em instantes.' :
        api.message;

      setError('root', { message });
    }
  }

  return (
    <main className="public-shell">
      <section className="auth-card" aria-labelledby="login-title">
        <div className="eyebrow">BarberShop</div>
        <h1 id="login-title">Bem-vindo de volta</h1>
        <p>Entre para acompanhar e gerenciar seus atendimentos.</p>

        {state?.accountCreated && (
          <div className="success-box" role="status">
            Conta criada com sucesso. Faça seu login.
          </div>
        )}

        <form onSubmit={handleSubmit(submit)} noValidate>
          <label htmlFor="login">Login</label>
          <input
            id="login"
            autoComplete="username"
            aria-invalid={!!errors.login}
            aria-describedby={errors.login ? 'login-error' : undefined}
            {...register('login')}
          />
          {errors.login && <span id="login-error">{errors.login.message}</span>}

          <label htmlFor="senha">Senha</label>
          <input
            id="senha"
            type="password"
            autoComplete="current-password"
            aria-invalid={!!errors.senha}
            aria-describedby={errors.senha ? 'senha-error' : undefined}
            {...register('senha')}
          />
          {errors.senha && <span id="senha-error">{errors.senha.message}</span>}

          {errors.root && <div className="form-error" role="alert">{errors.root.message}</div>}

          <button className="button primary" disabled={loginMutation.isPending}>
            {loginMutation.isPending ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="auth-links">
          <Link to="/novo">Criar conta</Link>
          <Link to="/esqueci-senha">Esqueci minha senha</Link>
        </div>
      </section>
    </main>
  );
}
