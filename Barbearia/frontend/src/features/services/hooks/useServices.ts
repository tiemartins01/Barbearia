import { useQuery } from '@tanstack/react-query'; import { getActiveServices } from '../api/servicesApi';
export const servicesKey=['services'] as const;
export function useServicesQuery(){ return useQuery({queryKey:servicesKey, queryFn:({signal})=>getActiveServices(signal), staleTime:5*60_000}); }
