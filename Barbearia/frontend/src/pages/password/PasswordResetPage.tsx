import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { usePasswordResetMutation } from '../../features/password-recovery/hooks/usePasswordRecovery';
import { messageFor } from '../../shared/api/apiError';

const schema = z.object({
  email: z.string().trim().email('E-mail inválido.'),
  codigo: z.string().trim().min(1, 'Informe o código.'),
  senha: z.string().min(6, 'Senha deve ter ao menos 6 caracteres.'),
  senhaRepetida: z.string(),
}).refine((v) => v.senha === v.senhaRepetida, {
  path: ['senhaRepetida'],
  message: 'As senhas não conferem.',
});

type Form = z.infer<typeof schema>;

export function PasswordResetPage() {
  const navigate = useNavigate();
  const mutation = usePasswordResetMutation();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setError,
  } = useForm<Form>({ resolver: zodResolver(schema) });

  async function submit(data: Form) {
    try {
      await mutation.mutateAsync(data);
      navigate('/login', { replace: true, state: { passwordReset: true } });
    } catch (error) {
      setError('root', { message: messageFor(error, 'Não foi possível redefinir a senha.') });
    }
  }

  return (
    <main className="public-shell">
      <section className="auth-card" aria-labelledby="reset-title">
        <div className="eyebrow">Nova senha</div>
        <h1 id="reset-title">Redefina sua senha</h1>

        <form onSubmit={handleSubmit(submit)} noValidate>
          <label htmlFor="reset-email">E-mail</label>
          <input id="reset-email" type="email" autoComplete="email" aria-invalid={!!errors.email} {...register('email')} />
          {errors.email && <span role="alert">{errors.email.message}</span>}

          <label htmlFor="reset-code">Código</label>
          <input id="reset-code" inputMode="numeric" autoComplete="one-time-code" aria-invalid={!!errors.codigo} {...register('codigo')} />
          {errors.codigo && <span role="alert">{errors.codigo.message}</span>}

          <label htmlFor="reset-password">Nova senha</label>
          <input id="reset-password" type="password" autoComplete="new-password" aria-invalid={!!errors.senha} {...register('senha')} />
          {errors.senha && <span role="alert">{errors.senha.message}</span>}

          <label htmlFor="reset-repeat">Repita a senha</label>
          <input id="reset-repeat" type="password" autoComplete="new-password" aria-invalid={!!errors.senhaRepetida} {...register('senhaRepetida')} />
          {errors.senhaRepetida && <span role="alert">{errors.senhaRepetida.message}</span>}

          {errors.root && <div className="form-error" role="alert">{errors.root.message}</div>}

          <button className="button primary" disabled={mutation.isPending}>
            {mutation.isPending ? 'Salvando...' : 'Redefinir senha'}
          </button>
        </form>

        <div className="auth-links"><Link to="/login">Voltar ao login</Link></div>
      </section>
    </main>
  );
}
