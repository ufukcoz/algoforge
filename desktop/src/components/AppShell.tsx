import type { ReactNode } from 'react';
import { useAuth } from '../context/AuthContext';

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
  const { username, logout } = useAuth();

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
};
