import axios from 'axios';

import type { ApiErrorResponse } from '../contracts/api';

export class ApiError extends Error {
  public readonly status?: number;
  public readonly code: string;
  public readonly traceId?: string;

  constructor(
    message: string,
    status?: number,
    code = 'UNKNOWN_ERROR',
    traceId?: string,
  ) {
    super(message);

    this.name = 'ApiError';
    this.status = status;
    this.code = code;
    this.traceId = traceId;
  }
}

export function normalizeApiError(error: unknown): ApiError {
  if (error instanceof ApiError) return error;

  if (!axios.isAxiosError<ApiErrorResponse>(error)) {
    return new ApiError('Ocorreu um erro inesperado.');
  }

  if (error.code === 'ECONNABORTED') {
    return new ApiError(
      'A API demorou demais para responder.Atenção',
      undefined,
      'TIMEOUT',
    );
  }

  if (!error.response) {
    return new ApiError(
      'Não foi possível conectar à API.',
      undefined,
      'NETWORK_ERROR',
    );
  }

  const body = error.response.data;

  return new ApiError(
    body?.mensagem ??
      body?.erro ??
      body?.message ??
      fallback(error.response.status),
    error.response.status,
    body?.codigo ?? `HTTP_${error.response.status}`,
    body?.traceId,
  );
}

function fallback(status: number) {
  if (status === 400) return 'A requisição enviada é inválida.';
  if (status === 401) return 'Sua autenticação não é válida.';
  if (status === 403) return 'Você não possui permissão para esta ação.';
  if (status === 404) return 'O recurso solicitado não foi encontrado.';
  if (status === 409) return 'A operação entrou em conflito com o estado atual.';
  if (status === 429) return 'Muitas tentativas. Aguarde e tente novamente.';
  if (status >= 500) return 'O servidor encontrou um erro inesperado.';

  return 'Não foi possível concluir a operação.';
}

export function messageFor(
  error: unknown,
  fallbackMessage: string,
) {
  const api = normalizeApiError(error);

  if (api.code === 'NETWORK_ERROR') {
    return 'Servidor indisponível. Verifique a conexão e tente novamente.';
  }

  if (api.code === 'TIMEOUT') {
    return 'A requisição demorou demais. Tente novamente.';
  }

  return api.message || fallbackMessage;
}