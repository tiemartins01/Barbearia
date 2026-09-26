import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { limparCsrfToken, obterCsrfToken } from './csrf';

const apiUrl = import.meta.env.VITE_API_URL;
if (!apiUrl) throw new Error('VITE_API_URL não foi configurada. Copie .env.example para .env.local.');

export const httpClient = axios.create({
  baseURL: apiUrl,
  withCredentials: true,
  timeout: 12_000,
  headers: { 'Content-Type': 'application/json' },
});

type RetryConfig = InternalAxiosRequestConfig & { _retry?: boolean };
let refreshPromise: Promise<void> | null = null;

function isMutation(method?: string) {
  return ['post', 'put', 'patch', 'delete'].includes((method ?? '').toLowerCase());
}

httpClient.interceptors.request.use(async (config) => {
  if (isMutation(config.method)) {
    const token = await obterCsrfToken();
    config.headers.set('X-CSRF-TOKEN', token);
  }
  return config;
});

async function refreshSession() {
  if (!refreshPromise) {
    refreshPromise = (async () => {
      const token = await obterCsrfToken();
      await axios.post(`${apiUrl}/auth/refresh`, undefined, {
        withCredentials: true,
        timeout: 12_000,
        headers: { 'X-CSRF-TOKEN': token },
      });
    })().finally(() => { refreshPromise = null; });
  }
  return refreshPromise;
}

httpClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as RetryConfig | undefined;
    const url = original?.url ?? '';
    const excluded = ['/auth/login', '/auth/refresh', '/password/recovery', '/password/reset', '/users'].some((p) => url.endsWith(p));

    if (error.response?.status === 401 && original && !original._retry && !excluded) {
      original._retry = true;
      try {
        await refreshSession();
        return httpClient(original);
      } catch {
        limparCsrfToken();
        window.dispatchEvent(new Event('auth:expired'));
      }
    }
    return Promise.reject(error);
  },
);
