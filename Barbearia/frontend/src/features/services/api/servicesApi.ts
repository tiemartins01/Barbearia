import { httpClient } from '../../../shared/api/httpClient';
import type { ServiceResponse } from '../../../shared/contracts/services';
export async function getActiveServices(signal?: AbortSignal) { const { data } = await httpClient.get<ServiceResponse[]>('/services', { signal }); return data; }
