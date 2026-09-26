import { useMutation, useQueryClient } from '@tanstack/react-query';
import { getCurrentUser, login, logout } from '../api/authApi';
import { useAuthStore } from '../model/authStore';
import type { LoginRequest } from '../../../shared/contracts/auth';

export function useLoginMutation() {
  const setUser = useAuthStore((s) => s.setUser);
  const setInitialized = useAuthStore((s) => s.setInitialized);

  return useMutation({
    mutationFn: async (request: LoginRequest) => {
      await login(request);
      return getCurrentUser();
    },
    onSuccess: (user) => {
      setUser(user);
      setInitialized(true);
    },
  });
}

export function useLogoutMutation() {
  const queryClient = useQueryClient();
  const clearSession = useAuthStore((s) => s.clearSession);

  return useMutation({
    mutationFn: logout,
    onSettled: async () => {
      queryClient.clear();
      clearSession();
    },
  });
}
