import { useCallback, useMemo, useState, type ReactNode } from 'react';
import {
  clearStoredAccessToken,
  getStoredAccessToken,
  setStoredAccessToken,
} from './tokenStorage';
import { AuthContext } from './authContext';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(getStoredAccessToken);

  const login = useCallback((accessToken: string) => {
    setStoredAccessToken(accessToken);
    setToken(accessToken);
  }, []);

  const logout = useCallback(() => {
    clearStoredAccessToken();
    setToken(null);
  }, []);

  const value = useMemo(
    () => ({
      isAuthenticated: Boolean(token),
      login,
      logout,
    }),
    [token, login, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
