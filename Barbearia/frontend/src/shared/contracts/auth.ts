export type UserRole = 'Admin' | 'Barbeiro' | 'Cliente';
export type LoginRequest = { nome: string; senha: string };
export type CurrentUserResponse = { id: number; nome: string; role: UserRole };
