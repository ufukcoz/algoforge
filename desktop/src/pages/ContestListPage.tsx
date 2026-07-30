import { useEffect, useState } from 'react';
import { getContests, joinContest, type ContestSummary } from '../api/client';
import { useAuth } from '../context/AuthContext';

interface ContestListPageProps {
  onSelectContest: (id: string) => void;
}

export default function ContestListPage({ onSelectContest }: ContestListPageProps) {
  const { accessToken } = useAuth();
  const [contests, setContests] = useState<ContestSummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [joiningId, setJoiningId] = useState<string | null>(null);

  const loadContests = () => {
    if (!accessToken) return;
    setIsLoading(true);
    getContests(accessToken)
      .then(setContests)
      .catch((err) => setError(err instanceof Error ? err.message : 'Yarismalar yuklenemedi.'))
      .finally(() => setIsLoading(false));
  };

  useEffect(loadContests, [accessToken]);

  const handleJoin = async (contest: ContestSummary) => {
    if (!accessToken) return;

    let inviteCode: string | null = null;
    if (!contest.isPublic) {
      inviteCode = window.prompt('Bu ozel bir yarisma. Davet kodunu gir:');
      if (!inviteCode) return;
    }

    setJoiningId(contest.id);
    setError(null);
    try {
      await joinContest(contest.id, inviteCode, accessToken);
      loadContests();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Katilim basarisiz.');
    } finally {
      setJoiningId(null);
    }
  };

  return (
    <div style={styles.page}>
      <p style={styles.eyebrow}>{'// yarismalar'}</p>
      <h1 style={styles.title}>Yarismalar</h1>

      {isLoading && <p style={styles.statusText}>yukleniyor...</p>}
      {error && <div style={styles.errorBox}>{error}</div>}

      {!isLoading && !error && (
        <div style={styles.list}>
          {contests.map((contest) => (
            <div key={contest.id} style={styles.card}>
              <div style={styles.cardMain} onClick={() => onSelectContest(contest.id)}>
                <div style={styles.cardTitleRow}>
                  <span style={styles.cardTitle}>{contest.title}</span>
                  <StatusBadge status={contest.status} />
                </div>
                <div style={styles.cardMeta}>
                  <span>{contest.questionCount} soru</span>
                  <span>{'\u2022'}</span>
                  <span>{contest.participantCount} katilimci</span>
                  <span>{'\u2022'}</span>
                  <span>{contest.isPublic ? 'public' : 'private'}</span>
                </div>
                <div style={styles.cardTime}>
                  {new Date(contest.startTime).toLocaleString('tr-TR')} {'\u2192'}{' '}
                  {new Date(contest.endTime).toLocaleString('tr-TR')}
                </div>
              </div>

              {!contest.isJoined && (
                <button
                  type="button"
                  onClick={() => handleJoin(contest)}
                  disabled={joiningId === contest.id}
                  style={styles.joinButton}
                >
                  {joiningId === contest.id ? 'katiliniyor...' : 'katil'}
                </button>
              )}
              {contest.isJoined && <span style={styles.joinedTag}>katildin</span>}
            </div>
          ))}
          {contests.length === 0 && (
            <p style={styles.statusText}>Henuz hicbir yarisma yok.</p>
          )}
        </div>
      )}
    </div>
  );
}

function StatusBadge({ status }: { status: ContestSummary['status'] }) {
  const config = {
    Active: { label: 'aktif', color: 'var(--color-success)' },
    Upcoming: { label: 'yakinda', color: 'var(--color-warning)' },
    Ended: { label: 'bitti', color: 'var(--color-text-muted)' },
  }[status];

  return (
    <span style={{ ...styles.statusBadge, color: config.color, borderColor: config.color }}>
      {config.label}
    </span>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    padding: '28px 32px',
    maxWidth: 720,
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
    marginBottom: 16,
  },
  list: {
    display: 'flex',
    flexDirection: 'column',
    gap: 10,
  },
  card: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: 16,
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: '14px 18px',
  },
  cardMain: {
    flex: 1,
    cursor: 'pointer',
  },
  cardTitleRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 10,
    marginBottom: 6,
  },
  cardTitle: {
    fontSize: 15,
    fontWeight: 600,
  },
  statusBadge: {
    fontFamily: 'var(--font-mono)',
    fontSize: 10,
    fontWeight: 700,
    padding: '2px 8px',
    borderRadius: 4,
    border: '1px solid',
    textTransform: 'uppercase',
  },
  cardMeta: {
    display: 'flex',
    gap: 8,
    fontSize: 12,
    color: 'var(--color-text-muted)',
    marginBottom: 4,
  },
  cardTime: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
  },
  joinButton: {
    background: 'var(--color-primary)',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '8px 16px',
    fontSize: 12,
    fontWeight: 600,
    cursor: 'pointer',
    whiteSpace: 'nowrap',
  },
  joinedTag: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-success)',
    whiteSpace: 'nowrap',
  },
};
