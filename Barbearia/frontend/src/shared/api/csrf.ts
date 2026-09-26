import axios from 'axios';

const apiUrl = import.meta.env.VITE_API_URL;
if (!apiUrl) throw new Error('VITE_API_URL não foi configurada.');
const csrfClient = axios.create({ baseURL: apiUrl, withCredentials: true, timeout: 12_000 });
let csrfToken: string | null = null;
let csrfPromise: Promise<string> | null = null;

export async function obterCsrfToken(force = false): Promise<string> {
  if (!force && csrfToken) return csrfToken;
  if (!force && csrfPromise) return csrfPromise;
  csrfPromise = csrfClient.get<{ token: string }>('/security/csrf')
    .then(({ data }) => {
      csrfToken = data.token;
      return data.token;
    })
    .finally(() => { csrfPromise = null; });
  return csrfPromise;
}
export function limparCsrfToken() { csrfToken = null; csrfPromise = null; }
