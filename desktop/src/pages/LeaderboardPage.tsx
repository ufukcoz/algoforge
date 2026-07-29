import { useEffect, useState } from 'react';
import { getLeaderboard, type LeaderboardEntry } from '../api/client';
import { useAuth } from '../context/AuthContext';

export default function LeaderboardPage() {
  const { username: currentUsername } = useAuth();
  const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setIsLoading(true);
    getLeaderboard(50)
      .then(setEntries)
      .catch((err) => setError(err instanceof Error ? err.message : 'Liderlik tablosu yuklenemedi.'))
      .finally(() => setIsLoading(false));
  }, []);

  return (
    <div style={styles.page}>
      <p style={styles.eyebrow}>{'// XP siralamasi'}</p>
      <h1 style={styles.title}>Liderlik Tablosu</h1>

      {isLoading && <p style={styles.statusText}>yukleniyor...</p>}
      {error && <div style={styles.errorBox}>{error}</div>}

      {!isLoading && !error && (
        <div style={styles.list}>
          {entries.map((entry) => {
            const isCurrentUser = entry.username === currentUsername;
            return (
              <div
                key={entry.rank}
                style={{
                  ...styles.row,
                  ...(isCurrentUser ? styles.rowHighlighted : {}),
                }}
              >
                <span style={styles.rank}>
                  {entry.rank <= 3 ? RANK_ICONS[entry.rank - 1] : `#${entry.rank}`}
                </span>
                <span style={styles.entryUsername}>{entry.username}</span>
                <span style={styles.entryLevel}>Lv.{entry.level}</span>
                <span style={styles.entryXp}>{entry.xp} XP</span>
              </div>
            );
          })}
          {entries.length === 0 && (
            <p style={styles.statusText}>Henuz kimse XP kazanmamis.</p>
          )}
        </div>
      )}
    </div>
  );
}

const RANK_ICONS = ['🥇', '🥈', '🥉'];

const styles: Record<string, React.CSSProperties> = {
  page: {
    padding: '28px 32px',
    maxWidth: 560,
  },
  eyebrow: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-primary)',
    margin: 0,
  },
  title: {
    fontSize: 24,
    margin: '8px 0 20px',
  },
  statusText: {
    color: 'var(--color-text-muted)',
    fontSize: 14,
  },
  errorBox: {
    background: 'rgba(239,68,68,0.1)',
    border: '1px solid rgba(239,68,68,0.35)',
    borderRadius: 6,
    padding: '10px 12px',
    fontSize: 13,
    color: '#fca5a5',
  },
  list: {
    display: 'flex',
    flexDirection: 'column',
    gap: 6,
  },
  row: {
    display: 'flex',
    alignItems: 'center',
    gap: 14,
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: '10px 16px',
  },
  rowHighlighted: {
    borderColor: 'var(--color-primary)',
    background: 'rgba(79,70,229,0.08)',
  },
  rank: {
    fontFamily: 'var(--font-mono)',
    fontSize: 14,
    width: 32,
    color: 'var(--color-text-muted)',
  },
  entryUsername: {
    flex: 1,
    fontSize: 14,
    fontWeight: 500,
  },
  entryLevel: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
    background: 'var(--color-bg)',
    padding: '3px 8px',
    borderRadius: 4,
  },
  entryXp: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    fontWeight: 600,
    color: 'var(--color-primary)',
    width: 70,
    textAlign: 'right',
  },
};
