// src/shared/api/csrf.ts

import { httpClient } from './httpClient';

let csrfToken: string | null = null;

export async function obterCsrfToken(): Promise<string> {
  if (csrfToken) {
    return csrfToken;
  }

  const response = await httpClient.get<{ token: string }>('/csrf');

  csrfToken = response.data.token;

  return csrfToken;
}

export function limparCsrfToken(): void {
  csrfToken = null;
}