import { useAuth } from '../context/AuthContext';

export default function DashboardPage() {
  const { username, accessToken, logout } = useAuth();

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        <p style={styles.eyebrow}>// oturum doğrulandı</p>
        <h1 style={styles.title}>
          Hoş geldin, <span style={styles.highlight}>{username}</span>
        </h1>
        <p style={styles.subtitle}>
          Backend'den gerçek bir JWT alındı. Question modülü henüz yok — bu ekran sadece
          uçtan uca bağlantının çalıştığını doğruluyor.
        </p>

        <div style={styles.tokenBox}>
          <span style={styles.tokenLabel}>accessToken</span>
          <code style={styles.tokenValue}>{accessToken?.slice(0, 48)}…</code>
        </div>

        <button type="button" onClick={logout} style={styles.logoutButton}>
          Çıkış yap
        </button>
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: '100%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 24,
  },
  card: {
    width: '100%',
    maxWidth: 520,
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 10,
    padding: 32,
  },
  eyebrow: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-success)',
    margin: 0,
  },
  title: {
    fontSize: 26,
    margin: '10px 0 8px',
  },
  highlight: {
    color: 'var(--color-primary)',
  },
  subtitle: {
    color: 'var(--color-text-muted)',
    fontSize: 14,
    lineHeight: 1.6,
  },
  tokenBox: {
    marginTop: 20,
    padding: '12px 14px',
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    borderRadius: 6,
    display: 'flex',
    flexDirection: 'column',
    gap: 6,
  },
  tokenLabel: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
  },
  tokenValue: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-text)',
    wordBreak: 'break-all',
  },
  logoutButton: {
    marginTop: 20,
    background: 'transparent',
    border: '1px solid var(--color-border)',
    color: 'var(--color-text)',
    borderRadius: 6,
    padding: '9px 16px',
    fontSize: 13,
    cursor: 'pointer',
  },
};
