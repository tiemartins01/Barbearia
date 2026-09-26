import { httpClient } from '../../../shared/api/httpClient'; import type { CreateReviewRequest } from '../../../shared/contracts/reviews';
export async function createReview(request:CreateReviewRequest){ const {data}=await httpClient.post<CreateReviewRequest>('/reviews',request); return data; }
