import { zodResolver } from '@hookform/resolvers/zod';
import type { ReactNode } from 'react';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { useRegisterClientMutation } from '../../features/registration/hooks/useRegistration';
import { normalizeApiError } from '../../shared/api/apiError';

const onlyDigits = (value: string) => value.replace(/\D/g, '');

const schema = z.object({
  nome: z.string().trim().min(2, 'Informe seu nome.'),
  email: z.string().trim().email('E-mail inválido.'),
  telefone: z.string().transform(onlyDigits).refine((v) => v.length === 11, 'Telefone deve ter 11 dígitos.'),
  cpf: z.string().transform(onlyDigits).refine((v) => v.length === 11, 'CPF deve ter 11 dígitos.'),
  login: z.string().trim().min(3, 'Login deve ter ao menos 3 caracteres.'),
  senha: z.string().min(6, 'Senha deve ter ao menos 6 caracteres.'),
  confirmar: z.string(),
}).refine((v) => v.senha === v.confirmar, {
  path: ['confirmar'],
  message: 'As senhas não conferem.',
});

type Form = z.infer<typeof schema>;

export function RegistrationPage() {
  const navigate = useNavigate();
  const mutation = useRegisterClientMutation();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setError,
  } = useForm<Form>({ resolver: zodResolver(schema) });

  async function submit(data: Form) {
    try {
      await mutation.mutateAsync({
        nome: data.nome,
        email: data.email,
        telefone: data.telefone,
        cpf: data.cpf,
        login: data.login,
        senha: data.senha,
        foto: null,
      });

      navigate('/login', { replace: true, state: { accountCreated: true } });
    } catch (error) {
      const api = normalizeApiError(error);
      setError('root', {
        message:
          api.status === 409
            ? api.message || 'Já existe um cadastro com um dos dados informados.'
            : api.message,
      });
    }
  }

  return (
    <main className="public-shell">
      <section className="auth-card wide" aria-labelledby="register-title">
        <div className="eyebrow">Nova conta</div>
        <h1 id="register-title">Crie seu acesso</h1>

        <form className="form-grid" onSubmit={handleSubmit(submit)} noValidate>
          <Field id="cad-nome" label="Nome" error={errors.nome?.message}>
            <input id="cad-nome" autoComplete="name" aria-invalid={!!errors.nome} {...register('nome')} />
          </Field>

          <Field id="cad-email" label="E-mail" error={errors.email?.message}>
            <input id="cad-email" type="email" autoComplete="email" aria-invalid={!!errors.email} {...register('email')} />
          </Field>

          <Field id="cad-phone" label="Telefone" error={errors.telefone?.message}>
            <input id="cad-phone" inputMode="numeric" autoComplete="tel" aria-invalid={!!errors.telefone} {...register('telefone')} />
          </Field>

          <Field id="cad-cpf" label="CPF" error={errors.cpf?.message}>
            <input id="cad-cpf" inputMode="numeric" aria-invalid={!!errors.cpf} {...register('cpf')} />
          </Field>

          <Field id="cad-login" label="Login" error={errors.login?.message}>
            <input id="cad-login" autoComplete="username" aria-invalid={!!errors.login} {...register('login')} />
          </Field>

          <Field id="cad-senha" label="Senha" error={errors.senha?.message}>
            <input id="cad-senha" type="password" autoComplete="new-password" aria-invalid={!!errors.senha} {...register('senha')} />
          </Field>

          <Field id="cad-confirmar" label="Confirmar senha" error={errors.confirmar?.message}>
            <input id="cad-confirmar" type="password" autoComplete="new-password" aria-invalid={!!errors.confirmar} {...register('confirmar')} />
          </Field>

          {errors.root && <div className="form-error full" role="alert">{errors.root.message}</div>}

          <button className="button primary full" disabled={mutation.isPending}>
            {mutation.isPending ? 'Criando...' : 'Criar conta'}
          </button>
        </form>

        <div className="auth-links"><Link to="/login">Voltar ao login</Link></div>
      </section>
    </main>
  );
}

function Field({
  id,
  label,
  error,
  children,
}: {
  id: string;
  label: string;
  error?: string;
  children: ReactNode;
}) {
  return (
    <div className="field">
      <label htmlFor={id}>{label}</label>
      {children}
      {error && <span role="alert">{error}</span>}
    </div>
  );
}
