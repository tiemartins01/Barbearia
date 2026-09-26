import { useMutation, useQueryClient } from '@tanstack/react-query'; import { createReview } from '../api/reviewsApi'; import { historyKey } from '../../appointments/hooks/useAppointments';
export function useCreateReviewMutation(){const q=useQueryClient();return useMutation({mutationFn:createReview,onSuccess:()=>q.invalidateQueries({queryKey:historyKey})});}
