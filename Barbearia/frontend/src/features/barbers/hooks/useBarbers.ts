import { useQuery } from '@tanstack/react-query'; import { getBarbers } from '../api/barbersApi';
export const barbersKey=['barbers'] as const;
export function useBarbersQuery(){return useQuery({queryKey:barbersKey,queryFn:({signal})=>getBarbers(signal),staleTime:2*60_000});}
