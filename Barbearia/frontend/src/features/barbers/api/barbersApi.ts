import { httpClient } from '../../../shared/api/httpClient'; import type { BarberResponse } from '../../../shared/contracts/barbers';
export async function getBarbers(signal?: AbortSignal){ const {data}=await httpClient.get<BarberResponse[]>('/barbers',{signal}); return data; }
