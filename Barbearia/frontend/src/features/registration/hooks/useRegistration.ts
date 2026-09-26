import { useMutation } from '@tanstack/react-query';
import { registerClient } from '../api/registrationApi';

export function useRegisterClientMutation() {
  return useMutation({
    mutationFn: registerClient,
    retry: 0,
  });
}
