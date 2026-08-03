import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios';

const apiUrl = import.meta.env.VITE_API_URL;
if (!apiUrl) throw new Error('VITE_API_URL não foi configurada. Copie .env.example para .env.local.');

export const httpClient = axios.create({
  baseURL: apiUrl,
  withCredentials: true,
  headers: { 'Content-Type': 'application/json' },
});

type RetryConfig = InternalAxiosRequestConfig & { _retry?: boolean };

httpClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as RetryConfig | undefined;
    const url = original?.url ?? '';
    const isAuthEndpoint = ['/login', '/login/me', '/login/refresh', '/login/logout'].some((path) => url.endsWith(path));

    if (error.response?.status === 401 && original && !original._retry && !isAuthEndpoint) {
      original._retry = true;
      try {
        await httpClient.post('/login/refresh');
        return httpClient(original);
      } catch {
        window.location.replace('/login');
      }
    }

    return Promise.reject(error);
  },
);
