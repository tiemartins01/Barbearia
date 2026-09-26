export type AvailableSlotsQuery = { id_barbeiro: number; id_servico: number; data: string };
export type AvailableSlotResponse = string;
export type CreateAppointmentRequest = { id_barbeiro: number; id_servico: number; horario: string };
export type NextAppointmentResponse = {
  id?: number;
  nomeServico: string;
  nomeBarbeiro: string;
  horario: string;
};
export type AppointmentHistoryItemResponse = {
  id: number;
  nomeServico: string;
  nomeBarbeiro: string;
  valorServico: number;
  data: string;
  podeAvaliar: boolean;
};
