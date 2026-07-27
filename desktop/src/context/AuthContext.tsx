import { createContext, useContext, useState } from 'react';
import type { ReactNode } from 'react';

interface AuthState {
  username: string | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  setSession: (username: string, accessToken: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [username, setUsername] = useState<string | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);

  const setSession = (newUsername: string, newAccessToken: string) => {
    setUsername(newUsername);
    setAccessToken(newAccessToken);
  };

  const logout = () => {
    setUsername(null);
    setAccessToken(null);
  };

  return (
    <AuthContext.Provider
      value={{ username, accessToken, isAuthenticated: !!accessToken, setSession, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth, AuthProvider içinde kullanılmalı');
  return context;
}
