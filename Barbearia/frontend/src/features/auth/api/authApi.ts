import { httpClient } from '../../../shared/api/httpClient';
import type { AuthUser } from '../model/authStore';
export async function login(login:string,senha:string){await httpClient.post('/login',{nome:login,senha});}
export async function getCurrentUser(){const {data}=await httpClient.get<AuthUser>('/login/me');return data;}
export async function logout(){await httpClient.post('/login/logout');}
