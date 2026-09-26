export type PasswordRecoveryRequest = { email: string };
export type PasswordResetRequest = {
  email: string;
  codigo: string;
  senha: string;
  senhaRepetida: string;
};
