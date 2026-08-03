import { create } from 'zustand';

export type UserRole = 'Admin' | 'Barbeiro' | 'Cliente';
export type AuthUser = { id: number; nome: string; role: UserRole };

type AuthState = {
  user: AuthUser | null;
  initialized: boolean;
  setUser: (user: AuthUser | null) => void;
  setInitialized: (value: boolean) => void;
  clearSession: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  initialized: false,
  setUser: (user) => set({ user }),
  setInitialized: (initialized) => set({ initialized }),
  clearSession: () => set({ user: null, initialized: true }),
}));
