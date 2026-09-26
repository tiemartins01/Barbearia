import { httpClient } from '../../../shared/api/httpClient';
import type { ApiResult } from '../../../shared/contracts/api';
import type { AppointmentHistoryItemResponse, AvailableSlotResponse, AvailableSlotsQuery, CreateAppointmentRequest, NextAppointmentResponse } from '../../../shared/contracts/appointments';

export async function getAvailableSlots(query: AvailableSlotsQuery, signal?: AbortSignal){
  const {data}=await httpClient.get<AvailableSlotResponse[]>('/appointments/available-slots',{params:query,signal}); return data;
}
export async function createAppointment(request: CreateAppointmentRequest){
  const key=globalThis.crypto?.randomUUID?.() ?? `${Date.now()}-${Math.random()}`;
  const {data}=await httpClient.post<ApiResult>('/appointments',request,{headers:{'Idempotency-Key':key}}); return data;
}
export async function getNextAppointment(signal?: AbortSignal){ const {data}=await httpClient.get<NextAppointmentResponse|null>('/appointments/next',{signal}); return data; }
export async function getAppointmentHistory(page=1,pageSize=10,signal?:AbortSignal){ const {data}=await httpClient.get<AppointmentHistoryItemResponse[]>('/appointments/history',{params:{page,pageSize},signal}); return data; }
// O frontend fica pronto para o contrato; o backend atual ainda precisa publicar esta rota.
export async function cancelAppointment(id:number){ await httpClient.patch(`/appointments/${id}/cancel`); }
