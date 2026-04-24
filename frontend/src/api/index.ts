/**
 * Публичная точка входа для общего слоя: вход/регистрация и HTTP-клиент.
 * Транзакции: `features/transactions` (типы + `transactionsApi`).
 */
export { authApi } from './auth';
export type { LoginResponse, RegisterResponse } from './types/auth';
