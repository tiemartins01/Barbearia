import { httpClient } from '../../../shared/api/httpClient';
import type { CreateUserRequest } from '../../../shared/contracts/users';
export async function registerClient(request: CreateUserRequest) { await httpClient.post('/users', request); }
