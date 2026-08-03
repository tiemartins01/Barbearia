import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';
import type { PropsWithChildren } from 'react';
import { AuthBootstrap } from '../../features/auth/ui/AuthBootstrap';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, staleTime: 30_000 },
    mutations: { retry: 0 },
  },
});

export function AppProviders({ children }: PropsWithChildren) {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthBootstrap>{children}</AuthBootstrap>
      </BrowserRouter>
    </QueryClientProvider>
  );
}
