import { create } from 'zustand';
import type { CurrentUserResponse, UserRole } from '../../../shared/contracts/auth';
export type { UserRole } from '../../../shared/contracts/auth';
export type AuthUser = CurrentUserResponse;

type AuthState = {
  user: AuthUser | null;
  initialized: boolean;
  setUser: (user: AuthUser | null) => void;
  setInitialized: (value: boolean) => void;
  clearSession: () => void;
};
export const useAuthStore = create<AuthState>((set) => ({
  user: null, initialized: false,
  setUser: (user) => set({ user }),
  setInitialized: (initialized) => set({ initialized }),
  clearSession: () => set({ user: null, initialized: true }),
}));
