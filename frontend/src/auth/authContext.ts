import { createContext } from 'react';

export type AuthContextValue = {
  isAuthenticated: boolean;
  login: (accessToken: string) => void;
  logout: () => void;
};

export const AuthContext = createContext<AuthContextValue | null>(null);
