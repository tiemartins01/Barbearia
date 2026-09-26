import { httpClient } from '../../../shared/api/httpClient';
import type { PasswordRecoveryRequest, PasswordResetRequest } from '../../../shared/contracts/password';
export async function requestPasswordRecovery(request: PasswordRecoveryRequest) { await httpClient.post('/password/recovery', request); }
export async function resetPassword(request: PasswordResetRequest) { await httpClient.post('/password/reset', request); }
