import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { getProfile, updateProfile } from '../api/profileApi';
export const profileKey = ['profile'] as const;
export function useProfileQuery() { return useQuery({ queryKey: profileKey, queryFn: ({ signal }) => getProfile(signal) }); }
export function useUpdateProfileMutation() { const qc=useQueryClient(); return useMutation({ mutationFn: updateProfile, onSuccess: () => qc.invalidateQueries({queryKey: profileKey}) }); }
