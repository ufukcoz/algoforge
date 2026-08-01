import { createContext, useContext, useEffect, useState, useCallback, useRef } from 'react';
import type { ReactNode } from 'react';
import { refreshAccessToken, logoutRequest } from '../api/client';

interface AuthState {
  username: string | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  setSession: (username: string, accessToken: string, refreshToken: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

// Access token 15 dakikada bir sona eriyor (backend JwtService.cs ile eslesir).
// Suresi dolmadan biraz once (10 dakikada bir) sessizce yeniliyoruz ki kullanici
// calisirken hicbir zaman "401 Unauthorized" ile karsilasmasin.
const REFRESH_INTERVAL_MS = 10 * 60 * 1000;

export function AuthProvider({ children }: { children: ReactNode }) {
  const [username, setUsername] = useState<string | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [refreshToken, setRefreshToken] = useState<string | null>(null);
  // refreshToken'in en guncel halini interval callback'i icinde okuyabilmek icin
  // (closure'in eski deger yakalamasini onlemek icin) bir ref'te de tutuyoruz.
  const refreshTokenRef = useRef<string | null>(null);
  refreshTokenRef.current = refreshToken;

  const setSession = (newUsername: string, newAccessToken: string, newRefreshToken: string) => {
    setUsername(newUsername);
    setAccessToken(newAccessToken);
    setRefreshToken(newRefreshToken);
  };

  const logout = useCallback(() => {
    if (refreshTokenRef.current) {
      // Sunucu tarafinda da iptal etmeye calis, ama bunu beklemeden hemen local state'i temizle.
      logoutRequest(refreshTokenRef.current);
    }
    setUsername(null);
    setAccessToken(null);
    setRefreshToken(null);
  }, []);

  useEffect(() => {
    if (!refreshToken) return;

    const intervalId = setInterval(async () => {
      const currentRefreshToken = refreshTokenRef.current;
      if (!currentRefreshToken) return;

      try {
        const result = await refreshAccessToken(currentRefreshToken);
        setAccessToken(result.accessToken);
        setRefreshToken(result.refreshToken);
      } catch {
        // Refresh token da gecersiz/suresi dolmus - kullaniciyi guvenli sekilde cikis yaptir,
        // tekrar login olmasi gerekecek.
        logout();
      }
    }, REFRESH_INTERVAL_MS);

    return () => clearInterval(intervalId);
  }, [refreshToken, logout]);

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
