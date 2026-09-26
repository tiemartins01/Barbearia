import { useMemo, useState } from 'react';
import { useBarbersQuery } from '../../features/barbers/hooks/useBarbers';
import { useServicesQuery } from '../../features/services/hooks/useServices';
import {
  useAvailableSlotsQuery,
  useCreateAppointmentMutation,
} from '../../features/appointments/hooks/useAppointments';
import { normalizeApiError } from '../../shared/api/apiError';
import { EmptyState, ErrorState, LoadingState } from '../../shared/ui/AsyncStates';

function toApiDateTime(date: string, time: string) {
  return `${date}T${time.length === 5 ? `${time}:00` : time}`;
}

function localToday() {
  const now = new Date();
  const offset = now.getTimezoneOffset();
  return new Date(now.getTime() - offset * 60_000).toISOString().slice(0, 10);
}

export function AppointmentPage() {
  const services = useServicesQuery();
  const barbers = useBarbersQuery();
  const create = useCreateAppointmentMutation();

  const [service, setService] = useState(0);
  const [barber, setBarber] = useState(0);
  const [date, setDate] = useState('');
  const [slot, setSlot] = useState('');
  const [feedback, setFeedback] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const query = useMemo(
    () => service && barber && date
      ? { id_barbeiro: barber, id_servico: service, data: date }
      : null,
    [service, barber, date],
  );

  const slots = useAvailableSlotsQuery(query);

  async function confirm() {
    if (!slot || !query || create.isPending) return;

    setFeedback(null);
    try {
      const result = await create.mutateAsync({
        id_barbeiro: barber,
        id_servico: service,
        horario: toApiDateTime(date, slot),
      });

      setFeedback({ type: 'success', text: result.mensagem || 'Agendamento realizado.' });
      setSlot('');
      await slots.refetch();
    } catch (error) {
      const api = normalizeApiError(error);

      if (api.status === 409) {
        setSlot('');
        setFeedback({
          type: 'error',
          text: 'Esse horário não está mais disponível. Atualizamos a agenda para você escolher outro.',
        });
        await slots.refetch();
        return;
      }

      setFeedback({ type: 'error', text: api.message });
    }
  }

  return (
    <div className="page">
      <header className="page-header">
        <div>
          <div className="eyebrow">Agendamento</div>
          <h1>Monte seu horário.</h1>
          <p>O backend valida expediente, duração e conflitos.</p>
        </div>
      </header>

      <div className="wizard">
        <section className="panel">
          <h2>1. Serviço</h2>
          {services.isLoading ? (
            <LoadingState />
          ) : services.isError ? (
            <ErrorState message="Não foi possível carregar os serviços." onRetry={() => services.refetch()} />
          ) : !services.data?.length ? (
            <EmptyState title="Nenhum serviço ativo." />
          ) : (
            <div className="choice-list">
              {services.data.map((s) => (
                <button
                  type="button"
                  key={s.id}
                  onClick={() => {
                    setService(s.id);
                    setSlot('');
                    setFeedback(null);
                  }}
                  className={service === s.id ? 'choice selected' : 'choice'}
                >
                  <strong>{s.nomeServico}</strong>
                  <span>{s.duracao} min</span>
                </button>
              ))}
            </div>
          )}
        </section>

        <section className="panel">
          <h2>2. Barbeiro</h2>
          {barbers.isLoading ? (
            <LoadingState />
          ) : barbers.isError ? (
            <ErrorState message="Não foi possível carregar os barbeiros." onRetry={() => barbers.refetch()} />
          ) : !barbers.data?.length ? (
            <EmptyState title="Nenhum barbeiro disponível." />
          ) : (
            <div className="choice-list">
              {barbers.data.map((b) => (
                <button
                  type="button"
                  key={b.id}
                  onClick={() => {
                    setBarber(b.id);
                    setSlot('');
                    setFeedback(null);
                  }}
                  className={barber === b.id ? 'choice selected' : 'choice'}
                >
                  <strong>{b.nome}</strong>
                  <span>{b.especialidade}</span>
                </button>
              ))}
            </div>
          )}
        </section>

        <section className="panel">
          <h2>3. Data e horário</h2>
          <label className="standalone-label" htmlFor="appointment-date">Data</label>
          <input
            id="appointment-date"
            type="date"
            min={localToday()}
            value={date}
            onChange={(e) => {
              setDate(e.target.value);
              setSlot('');
              setFeedback(null);
            }}
          />

          {query && (slots.isLoading || slots.isFetching) ? (
            <LoadingState label="Consultando horários..." />
          ) : slots.isError ? (
            <ErrorState message="Não foi possível consultar os horários." onRetry={() => slots.refetch()} />
          ) : query && !slots.data?.length ? (
            <EmptyState title="Nenhum horário disponível." />
          ) : (
            <div className="slots">
              {slots.data?.map((time) => (
                <button
                  type="button"
                  key={time}
                  onClick={() => {
                    setSlot(time);
                    setFeedback(null);
                  }}
                  className={slot === time ? 'slot selected' : 'slot'}
                >
                  {time.slice(0, 5)}
                </button>
              ))}
            </div>
          )}
        </section>

        <section className="panel summary">
          <h2>4. Confirmar</h2>
          <p>{service && barber && date && slot ? 'Revise a seleção e confirme.' : 'Selecione serviço, barbeiro, data e horário.'}</p>

          {feedback && (
            <div className={feedback.type === 'success' ? 'success-box' : 'form-error'} role="status">
              {feedback.text}
            </div>
          )}

          <button
            type="button"
            className="button primary"
            onClick={confirm}
            disabled={!slot || create.isPending}
          >
            {create.isPending ? 'Agendando...' : 'Confirmar agendamento'}
          </button>
        </section>
      </div>
    </div>
  );
}
