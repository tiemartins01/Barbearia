import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { AvailableSlotsQuery } from '../../../shared/contracts/appointments';
import { cancelAppointment, createAppointment, getAppointmentHistory, getAvailableSlots, getNextAppointment } from '../api/appointmentsApi';
export const nextAppointmentKey=['next-appointment'] as const; export const historyKey=['appointment-history'] as const;
export function useAvailableSlotsQuery(q:AvailableSlotsQuery|null){return useQuery({queryKey:['available-slots',q?.id_barbeiro,q?.id_servico,q?.data],queryFn:({signal})=>getAvailableSlots(q!,signal),enabled:!!q&&q.id_barbeiro>0&&q.id_servico>0&&!!q.data,staleTime:10_000,refetchOnWindowFocus:true});}
export function useNextAppointmentQuery(){return useQuery({queryKey:nextAppointmentKey,queryFn:({signal})=>getNextAppointment(signal),staleTime:15_000});}
export function useHistoryQuery(page=1,pageSize=10){return useQuery({queryKey:[...historyKey,page,pageSize],queryFn:({signal})=>getAppointmentHistory(page,pageSize,signal)});}
export function useCreateAppointmentMutation(){const q=useQueryClient(); return useMutation({mutationFn:createAppointment,onSuccess:async()=>{await Promise.all([q.invalidateQueries({queryKey:['available-slots']}),q.invalidateQueries({queryKey:nextAppointmentKey}),q.invalidateQueries({queryKey:historyKey})]);}});}
export function useCancelAppointmentMutation(){const q=useQueryClient(); return useMutation({mutationFn:cancelAppointment,onSuccess:async()=>{await Promise.all([q.invalidateQueries({queryKey:nextAppointmentKey}),q.invalidateQueries({queryKey:historyKey}),q.invalidateQueries({queryKey:['available-slots']})]);}});}
