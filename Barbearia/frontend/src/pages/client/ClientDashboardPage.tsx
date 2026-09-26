import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useBarbersQuery } from '../../features/barbers/hooks/useBarbers';
import {
  useCancelAppointmentMutation,
  useNextAppointmentQuery,
} from '../../features/appointments/hooks/useAppointments';
import { useAuthStore } from '../../features/auth/model/authStore';
import { ErrorState, LoadingState } from '../../shared/ui/AsyncStates';
import { messageFor } from '../../shared/api/apiError';

export function ClientDashboardPage() {
  const user = useAuthStore((s) => s.user);
  const next = useNextAppointmentQuery();
  const barbers = useBarbersQuery();
  const cancel = useCancelAppointmentMutation();
  const [cancelError, setCancelError] = useState('');

  async function cancelNext(id: number) {
    if (!window.confirm('Deseja realmente cancelar este agendamento?')) return;

    setCancelError('');
    try {
      await cancel.mutateAsync(id);
    } catch (error) {
      setCancelError(messageFor(error, 'Não foi possível cancelar o agendamento.'));
    }
  }

  return (
    <div className="page">
      <header className="page-header">
        <div>
          <div className="eyebrow">Olá, {user?.nome}</div>
          <h1>Seu próximo corte começa aqui.</h1>
          <p>Agende, acompanhe e avalie seus atendimentos em um só lugar.</p>
        </div>
        <Link className="button primary" to="/cliente/agendar">Novo agendamento</Link>
      </header>

      <section>
        <div className="section-heading"><h2>Próximo agendamento</h2></div>

        {next.isLoading ? (
          <LoadingState />
        ) : next.isError ? (
          <ErrorState message={messageFor(next.error, 'Falha ao carregar.')} onRetry={() => next.refetch()} />
        ) : next.data ? (
          <article className="appointment-card">
            <div><span>Serviço</span><strong>{next.data.nomeServico}</strong></div>
            <div><span>Barbeiro</span><strong>{next.data.nomeBarbeiro}</strong></div>
            <div><span>Horário</span><strong>{next.data.horario}</strong></div>

            {next.data.id ? (
              <button
                className="button secondary"
                disabled={cancel.isPending}
                onClick={() => cancelNext(next.data!.id!)}
              >
                {cancel.isPending ? 'Cancelando...' : 'Cancelar'}
              </button>
            ) : (
              <small className="contract-note">
                O frontend já suporta cancelamento. O backend atual ainda precisa devolver o ID do próximo agendamento.
              </small>
            )}

            {cancelError && <div className="form-error" role="alert">{cancelError}</div>}
          </article>
        ) : (
          <div className="state-card">
            <strong>Nenhum agendamento futuro.</strong>
            <span>Escolha um serviço e marque seu próximo horário.</span>
          </div>
        )}
      </section>

      <section>
        <div className="section-heading"><h2>Barbeiros</h2><Link to="/cliente/agendar">Ver agenda</Link></div>
        {barbers.isLoading ? (
          <LoadingState />
        ) : barbers.isError ? (
          <ErrorState message={messageFor(barbers.error, 'Falha ao carregar barbeiros.')} onRetry={() => barbers.refetch()} />
        ) : !barbers.data?.length ? (
          <div className="state-card"><strong>Nenhum barbeiro disponível.</strong></div>
        ) : (
          <div className="card-grid">
            {barbers.data.slice(0, 3).map((b) => (
              <article className="barber-card" key={b.id}>
                <div className="avatar large">{b.iniciais || b.nome.slice(0, 2).toUpperCase()}</div>
                <h3>{b.nome}</h3>
                <span>{b.especialidade}</span>
                <small>★ {b.notaMedia.toFixed(1)} · {b.quantidadeAvaliacoes} avaliações</small>
              </article>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
