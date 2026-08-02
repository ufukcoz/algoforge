import { useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import { useAuth } from '../context/AuthContext';
import { getProfile, resendVerificationEmail } from '../api/client';

export type View = 'questions' | 'contests' | 'profile' | 'leaderboard';

interface AppShellProps {
  activeView: View;
  onNavigate: (view: View) => void;
  children: ReactNode;
}

const NAV_ITEMS: { id: View; label: string }[] = [
  { id: 'questions', label: 'Sorular' },
  { id: 'contests', label: 'Yarismalar' },
  { id: 'leaderboard', label: 'Liderlik Tablosu' },
  { id: 'profile', label: 'Profil' },
];

export default function AppShell({ activeView, onNavigate, children }: AppShellProps) {
  const { username, accessToken, logout } = useAuth();
  const [isEmailVerified, setIsEmailVerified] = useState<boolean | null>(null);
  const [isResending, setIsResending] = useState(false);
  const [resendMessage, setResendMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!accessToken) return;
    getProfile(accessToken)
      .then((profile) => setIsEmailVerified(profile.emailVerified))
      .catch(() => {
        // Profil yuklenemese bile uygulamayi kullanmaya devam edebilsin -
        // bu banner sadece bilgilendirici, kritik bir engel degil.
      });
  }, [accessToken]);

  const handleResend = async () => {
    if (!accessToken) return;
    setIsResending(true);
    setResendMessage(null);
    try {
      await resendVerificationEmail(accessToken);
      setResendMessage('Dogrulama emaili tekrar gonderildi, gelen kutunu kontrol et.');
    } catch {
      setResendMessage('Gonderilemedi, birazdan tekrar dene.');
    } finally {
      setIsResending(false);
    }
  };

  return (
    <div style={styles.page}>
      <header style={styles.header}>
        <div style={styles.brand}>
          <span style={styles.brandMark}>{'</>'}</span>
          <span style={styles.brandName}>AlgoForge</span>
        </div>

        <nav style={styles.nav}>
          {NAV_ITEMS.map((item) => (
            <button
              key={item.id}
              type="button"
              onClick={() => onNavigate(item.id)}
              style={{
                ...styles.navItem,
                ...(activeView === item.id ? styles.navItemActive : {}),
              }}
            >
              {item.label}
            </button>
          ))}
        </nav>

        <div style={styles.headerRight}>
          <span style={styles.username}>{username}</span>
          <button type="button" onClick={logout} style={styles.logoutButton}>
            Cikis yap
          </button>
        </div>
      </header>

      {isEmailVerified === false && (
        <div style={styles.verifyBanner}>
          <span>
            {'\u26a0\ufe0f'} E-posta adresin henuz dogrulanmadi.
            {resendMessage ? ` ${resendMessage}` : ' Gelen kutunu kontrol et.'}
          </span>
          <button
            type="button"
            onClick={handleResend}
            disabled={isResending}
            style={styles.verifyButton}
          >
            {isResending ? 'gonderiliyor...' : 'tekrar gonder'}
          </button>
        </div>
      )}

      <div style={styles.content}>{children}</div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
  },
  header: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '14px 24px',
    borderBottom: '1px solid var(--color-border)',
    background: 'var(--color-bg-elevated)',
  },
  brand: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
    fontFamily: 'var(--font-mono)',
  },
  brandMark: {
    color: 'var(--color-primary)',
    fontSize: 18,
    fontWeight: 700,
  },
  brandName: {
    fontSize: 15,
    fontWeight: 600,
  },
  nav: {
    display: 'flex',
    gap: 4,
  },
  navItem: {
    background: 'none',
    border: 'none',
    color: 'var(--color-text-muted)',
    fontSize: 13,
    padding: '7px 14px',
    borderRadius: 6,
    cursor: 'pointer',
  },
  navItemActive: {
    background: 'rgba(79,70,229,0.15)',
    color: 'var(--color-primary)',
    fontWeight: 600,
  },
  headerRight: {
    display: 'flex',
    alignItems: 'center',
    gap: 14,
  },
  username: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    color: 'var(--color-text-muted)',
  },
  logoutButton: {
    background: 'transparent',
    border: '1px solid var(--color-border)',
    color: 'var(--color-text)',
    borderRadius: 6,
    padding: '6px 12px',
    fontSize: 12,
    cursor: 'pointer',
  },
  content: {
    flex: 1,
    minHeight: 0,
    overflowY: 'auto',
  },
  verifyBanner: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: 12,
    padding: '10px 24px',
    background: 'rgba(245,158,11,0.12)',
    borderBottom: '1px solid rgba(245,158,11,0.35)',
    fontSize: 13,
    color: '#fcd34d',
  },
  verifyButton: {
    background: 'transparent',
    border: '1px solid #fcd34d',
    color: '#fcd34d',
    borderRadius: 6,
    padding: '5px 12px',
    fontSize: 12,
    cursor: 'pointer',
    whiteSpace: 'nowrap',
  },
};
