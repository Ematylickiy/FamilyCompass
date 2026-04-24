import { API_V1_PREFIX } from './http/client';
import type { LoginResponse, RegisterResponse } from './types/auth';

async function postAuth<T>(path: string, body: Record<string, unknown>): Promise<T> {
  const response = await fetch(`${API_V1_PREFIX}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  const text = await response.text();
  let message = text || `${response.status} ${response.statusText}`;
  if (text) {
    try {
      const parsed = JSON.parse(text) as { error?: string };
      if (parsed.error) {
        message = parsed.error;
      }
    } catch {
      /* keep raw text */
    }
  }
  if (!response.ok) {
    if (response.status === 401) {
      throw new Error('Неверное имя пользователя или пароль');
    }
    throw new Error(message);
  }
  if (!text) {
    return undefined as T;
  }
  return JSON.parse(text) as T;
}

/** Публичные эндпоинты входа и регистрации (без Bearer). */
export const authApi = {
  login: (username: string, password: string) =>
    postAuth<LoginResponse>('auth/login', { username, password }),

  register: (username: string, password: string, confirmPassword: string) =>
    postAuth<RegisterResponse>('auth/register', {
      username,
      password,
      confirmPassword,
    }),
};
