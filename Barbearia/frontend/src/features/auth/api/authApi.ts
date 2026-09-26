import { httpClient } from '../../../shared/api/httpClient';
import { limparCsrfToken } from '../../../shared/api/csrf';
import type { CurrentUserResponse, LoginRequest } from '../../../shared/contracts/auth';
export async function login(request: LoginRequest) { await httpClient.post('/auth/login', request); }
export async function getCurrentUser() { const { data } = await httpClient.get<CurrentUserResponse>('/auth/me'); return data; }
export async function logout() { await httpClient.post('/auth/logout'); limparCsrfToken(); }
