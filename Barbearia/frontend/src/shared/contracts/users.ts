export type CreateUserRequest = {
  nome: string;
  email: string;
  phone: string;
  cpf: string;
  login: string;
  senhaR: string;
  foto?: string | null;
};

export type UserProfileResponse = {
  id: number;
  nome: string;
  iniciais: string;
  email: string;
  qtdcortes: number;
  telefone: string;
  cpf: string;
};

export type UpdateUserProfileRequest = {
  id: number;
  nome: string;
  email: string;
  telefone: string;
  cpf: string;
  senhaAntiga: string;
  novaSenha: string;
};
