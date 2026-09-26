import { useMutation } from '@tanstack/react-query';
import { requestPasswordRecovery, resetPassword } from '../api/passwordApi';

export function usePasswordRecoveryMutation() {
  return useMutation({
    mutationFn: requestPasswordRecovery,
    retry: 0,
  });
}

export function usePasswordResetMutation() {
  return useMutation({
    mutationFn: resetPassword,
    retry: 0,
  });
}
