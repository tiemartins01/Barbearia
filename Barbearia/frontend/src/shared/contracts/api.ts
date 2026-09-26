export type ApiErrorResponse = {
  sucesso?: false;
  codigo?: string;
  mensagem?: string;
  traceId?: string;
  erro?: string;
  message?: string;
};

export type ApiResult = {
  sucesso: boolean;
  mensagem: string;
};
