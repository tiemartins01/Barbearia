import { useEffect, type PropsWithChildren } from 'react';
import { getCurrentUser } from '../api/authApi';
import { useAuthStore } from '../model/authStore';

export function AuthBootstrap({ children }: PropsWithChildren) {
  const initialized = useAuthStore((state) => state.initialized);
  const setUser = useAuthStore((state) => state.setUser);
  const setInitialized = useAuthStore((state) => state.setInitialized);

  useEffect(() => {
    if (initialized) return;

    getCurrentUser()
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setInitialized(true));
  }, [initialized, setInitialized, setUser]);

  return children;
}
