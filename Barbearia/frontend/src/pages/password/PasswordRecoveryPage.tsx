import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link } from 'react-router-dom';
import { z } from 'zod';
import { usePasswordRecoveryMutation } from '../../features/password-recovery/hooks/usePasswordRecovery';
import { messageFor } from '../../shared/api/apiError';

const schema = z.object({ email: z.string().trim().email('E-mail inválido.') });
type Form = z.infer<typeof schema>;

export function PasswordRecoveryPage() {
  const [sent, setSent] = useState(false);
  const mutation = usePasswordRecoveryMutation();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setError,
  } = useForm<Form>({ resolver: zodResolver(schema) });

  async function submit(data: Form) {
    try {
      await mutation.mutateAsync(data);
      setSent(true);
    } catch (error) {
      setError('root', { message: messageFor(error, 'Não foi possível solicitar a recuperação.') });
    }
  }

  return (
    <main className="public-shell">
      <section className="auth-card" aria-labelledby="recovery-title">
        <div className="eyebrow">Recuperar senha</div>
        <h1 id="recovery-title">Recupere seu acesso</h1>

        {sent ? (
          <>
            <div className="success-box" role="status">
              Se o e-mail estiver cadastrado, as instruções foram enviadas.
            </div>
            <Link className="button primary" to="/trocar">Já tenho o código</Link>
          </>
        ) : (
          <form onSubmit={handleSubmit(submit)} noValidate>
            <label htmlFor="recovery-email">E-mail</label>
            <input
              id="recovery-email"
              type="email"
              autoComplete="email"
              aria-invalid={!!errors.email}
              {...register('email')}
            />
            {errors.email && <span role="alert">{errors.email.message}</span>}
            {errors.root && <div className="form-error" role="alert">{errors.root.message}</div>}

            <button className="button primary" disabled={mutation.isPending}>
              {mutation.isPending ? 'Enviando...' : 'Enviar código'}
            </button>
          </form>
        )}

        <div className="auth-links"><Link to="/login">Voltar ao login</Link></div>
      </section>
    </main>
  );
}
