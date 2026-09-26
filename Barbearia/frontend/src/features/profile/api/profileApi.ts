import { httpClient } from '../../../shared/api/httpClient';
import type { UpdateUserProfileRequest, UserProfileResponse } from '../../../shared/contracts/users';
export async function getProfile(signal?: AbortSignal) { const { data } = await httpClient.get<UserProfileResponse>('/users/me', { signal }); return data; }
export async function updateProfile(request: UpdateUserProfileRequest) { await httpClient.patch('/users/me', request); }
