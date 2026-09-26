import { useEffect, type PropsWithChildren } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { getCurrentUser } from '../api/authApi';
import { useAuthStore } from '../model/authStore';

export function AuthBootstrap({ children }: PropsWithChildren) {
  const queryClient = useQueryClient();
  const initialized = useAuthStore((s) => s.initialized);
  const setUser = useAuthStore((s) => s.setUser);
  const setInitialized = useAuthStore((s) => s.setInitialized);
  const clearSession = useAuthStore((s) => s.clearSession);

  useEffect(() => {
    const expired = () => {
      queryClient.clear();
      clearSession();
    };

    window.addEventListener('auth:expired', expired);
    return () => window.removeEventListener('auth:expired', expired);
  }, [clearSession, queryClient]);

  useEffect(() => {
    if (initialized) return;

    getCurrentUser()
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setInitialized(true));
  }, [initialized, setInitialized, setUser]);

  return children;
}
