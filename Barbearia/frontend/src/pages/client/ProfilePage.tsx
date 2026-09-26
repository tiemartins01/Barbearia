import { useEffect } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import {
  useProfileQuery,
  useUpdateProfileMutation,
} from '../../features/profile/hooks/useProfile';

import { useAuthStore } from '../../features/auth/model/authStore';
import { ErrorState, LoadingState } from '../../shared/ui/AsyncStates';
import { messageFor } from '../../shared/api/apiError';

const digits = (v: string) => v.replace(/\D/g, '');

const schema = z
  .object({
    nome: z.string().trim().min(2, 'Nome deve ter ao menos 2 caracteres.'),

    email: z.string().email('E-mail inválido.'),

    telefone: z
      .string()
      .transform(digits)
      .refine(
        v => v.length === 11,
        'Telefone deve ter 11 dígitos.',
      ),

    cpf: z
      .string()
      .transform(digits)
      .refine(
        v => v.length === 11,
        'CPF deve ter 11 dígitos.',
      ),

    senhaAntiga: z.string(),

    novaSenha: z.string(),
  })
  .refine(
    v => !v.novaSenha || v.novaSenha.length >= 6,
    {
      path: ['novaSenha'],
      message: 'Nova senha deve ter ao menos 6 caracteres.',
    },
  )
  .refine(
    v => !v.novaSenha || !!v.senhaAntiga,
    {
      path: ['senhaAntiga'],
      message: 'Informe a senha atual.',
    },
  );

type Form = z.infer<typeof schema>;

export function ProfilePage() {
  const q = useProfileQuery();
  const mutation = useUpdateProfileMutation();

  const setUser = useAuthStore(s => s.setUser);
  const auth = useAuthStore(s => s.user);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
    setError,
  } = useForm<Form>({
    resolver: zodResolver(schema),
    defaultValues: {
      nome: '',
      email: '',
      telefone: '',
      cpf: '',
      senhaAntiga: '',
      novaSenha: '',
    },
  });

  useEffect(() => {
    if (q.data) {
      reset({
        nome: q.data.nome,
        email: q.data.email,
        telefone: q.data.telefone,
        cpf: q.data.cpf,
        senhaAntiga: '',
        novaSenha: '',
      });
    }
  }, [q.data, reset]);

  async function submit(d: Form) {
    try {
      await mutation.mutateAsync({
        id: 0,
        nome: d.nome,
        email: d.email,
        telefone: d.telefone,
        cpf: d.cpf,
        senhaAntiga: d.senhaAntiga,
        novaSenha: d.novaSenha,
      });

      if (auth) {
        setUser({
          ...auth,
          nome: d.nome,
        });
      }

      reset({
        ...d,
        senhaAntiga: '',
        novaSenha: '',
      });
    } catch (e) {
      setError('root', {
        message: messageFor(
          e,
          'Não foi possível atualizar o perfil.',
        ),
      });
    }
  }

  if (q.isLoading) {
    return (
      <div className="page">
        <LoadingState />
      </div>
    );
  }

  if (q.isError) {
    return (
      <div className="page">
        <ErrorState
          message={messageFor(
            q.error,
            'Falha ao carregar perfil.',
          )}
          onRetry={() => q.refetch()}
        />
      </div>
    );
  }

  return (
    <div className="page">
      <header className="page-header">
        <div>
          <div className="eyebrow">Perfil</div>

          <h1>Seus dados pessoais.</h1>

          <p>
            {q.data?.qtdcortes ?? 0} cortes registrados.
          </p>
        </div>
      </header>

      <form
        className="profile-form panel"
        onSubmit={handleSubmit(submit)}
      >
        <label>
          Nome

          <input {...register('nome')} />

          <span>{errors.nome?.message}</span>
        </label>

        <label>
          E-mail

          <input
            type="email"
            {...register('email')}
          />

          <span>{errors.email?.message}</span>
        </label>

        <label>
          Telefone

          <input {...register('telefone')} />

          <span>{errors.telefone?.message}</span>
        </label>

        <label>
          CPF

          <input {...register('cpf')} />

          <span>{errors.cpf?.message}</span>
        </label>

        <div className="form-separator">
          Alterar senha (opcional)
        </div>

        <label>
          Senha atual

          <input
            type="password"
            autoComplete="current-password"
            {...register('senhaAntiga')}
          />

          <span>{errors.senhaAntiga?.message}</span>
        </label>

        <label>
          Nova senha

          <input
            type="password"
            autoComplete="new-password"
            {...register('novaSenha')}
          />

          <span>{errors.novaSenha?.message}</span>
        </label>

        {errors.root && (
          <div className="form-error full">
            {errors.root.message}
          </div>
        )}

        {mutation.isSuccess && (
          <div className="success-box full">
            Dados atualizados com sucesso.
          </div>
        )}

        <button
          className="button primary full"
          disabled={mutation.isPending}
        >
          {mutation.isPending
            ? 'Salvando...'
            : 'Salvar alterações'}
        </button>
      </form>
    </div>
  );
}