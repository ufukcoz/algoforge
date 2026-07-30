import { useEffect, useState } from 'react';
import {
  getContestById,
  getContestLeaderboard,
  type ContestDetail,
  type ContestLeaderboardEntry,
} from '../api/client';
import { useAuth } from '../context/AuthContext';

interface ContestDetailPageProps {
  contestId: string;
  onBack: () => void;
  onSelectQuestion: (id: string) => void;
}

export default function ContestDetailPage({
  contestId,
  onBack,
  onSelectQuestion,
}: ContestDetailPageProps) {
  const { accessToken, username: currentUsername } = useAuth();
  const [contest, setContest] = useState<ContestDetail | null>(null);
  const [leaderboard, setLeaderboard] = useState<ContestLeaderboardEntry[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!accessToken) return;
    setIsLoading(true);
    setError(null);
    Promise.all([
      getContestById(contestId, accessToken),
      getContestLeaderboard(contestId, accessToken),
    ])
      .then(([contestData, leaderboardData]) => {
        setContest(contestData);
        setLeaderboard(leaderboardData);
      })
      .catch((err) => setError(err instanceof Error ? err.message : 'Yarisma yuklenemedi.'))
      .finally(() => setIsLoading(false));
  }, [contestId, accessToken]);

  return (
    <div style={styles.page}>
      <header style={styles.header}>
        <button type="button" onClick={onBack} style={styles.backButton}>
          {'\u2190'} Yarismalara don
        </button>
      </header>

      <main style={styles.main}>
        {isLoading && <p style={styles.statusText}>yukleniyor...</p>}
        {error && <div style={styles.errorBox}>{error}</div>}

        {contest && (
          <>
            <h1 style={styles.title}>{contest.title}</h1>
            <p style={styles.description}>{contest.description}</p>

            <div style={styles.metaRow}>
              <span style={styles.metaTag}>{contest.status}</span>
              <span style={styles.metaTag}>{contest.participantCount} katilimci</span>
              {contest.inviteCode && (
                <span style={styles.metaTag}>davet kodu: {contest.inviteCode}</span>
              )}
            </div>

            <h2 style={styles.sectionTitle}>{'// sorular'}</h2>
            <div style={styles.questionList}>
              {contest.questions.map((q) => (
                <button
                  key={q.questionId}
                  type="button"
                  onClick={() => onSelectQuestion(q.questionId)}
                  style={styles.questionRow}
                  disabled={!contest.isJoined}
                >
                  <span style={styles.questionTitle}>{q.title}</span>
                  <span style={styles.questionMeta}>
                    <DifficultyBadge difficulty={q.difficulty} />
                    <span style={styles.pointsTag}>{q.points} puan</span>
                  </span>
                </button>
              ))}
            </div>
            {!contest.isJoined && (
              <p style={styles.joinHint}>
                Sorulari cozmek icin once yarismaya katilman gerekiyor.
              </p>
            )}

            <h2 style={styles.sectionTitle}>{'// skor tablosu'}</h2>
            <div style={styles.leaderboardList}>
              {leaderboard.map((entry) => {
                const isCurrentUser = entry.username === currentUsername;
                return (
                  <div
                    key={entry.rank}
                    style={{
                      ...styles.leaderboardRow,
                      ...(isCurrentUser ? styles.leaderboardRowHighlighted : {}),
                    }}
                  >
                    <span style={styles.rank}>#{entry.rank}</span>
                    <span style={styles.entryUsername}>{entry.username}</span>
                    <span style={styles.entrySolved}>{entry.solvedCount} cozuldu</span>
                    <span style={styles.entryPoints}>{entry.totalPoints} puan</span>
                  </div>
                );
              })}
              {leaderboard.length === 0 && (
                <p style={styles.statusText}>Henuz kimse puan kazanmamis.</p>
              )}
            </div>
          </>
        )}
      </main>
    </div>
  );
}

function DifficultyBadge({ difficulty }: { difficulty: string }) {
  const colorVar =
    difficulty === 'Easy'
      ? 'var(--color-success)'
      : difficulty === 'Medium'
        ? 'var(--color-warning)'
        : 'var(--color-danger)';

  return (
    <span style={{ ...styles.difficultyBadge, color: colorVar, borderColor: colorVar }}>
      {difficulty}
    </span>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
  },
  header: {
    padding: '14px 24px',
    borderBottom: '1px solid var(--color-border)',
    background: 'var(--color-bg-elevated)',
  },
  backButton: {
    background: 'none',
    border: 'none',
    color: 'var(--color-text-muted)',
    fontSize: 13,
    cursor: 'pointer',
    padding: 0,
  },
  main: {
    flex: 1,
    padding: '28px 32px',
    overflowY: 'auto',
    maxWidth: 720,
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
  title: {
    fontSize: 24,
    margin: '0 0 8px',
  },
  description: {
    fontSize: 14,
    color: 'var(--color-text-muted)',
    marginBottom: 16,
  },
  metaRow: {
    display: 'flex',
    gap: 8,
    marginBottom: 28,
  },
  metaTag: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    padding: '3px 9px',
    borderRadius: 4,
  },
  sectionTitle: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    color: 'var(--color-primary)',
    marginBottom: 12,
  },
  questionList: {
    display: 'flex',
    flexDirection: 'column',
    gap: 8,
    marginBottom: 8,
  },
  questionRow: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: '12px 16px',
    cursor: 'pointer',
    textAlign: 'left',
  },
  questionTitle: {
    fontSize: 14,
    fontWeight: 500,
  },
  questionMeta: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
  },
  difficultyBadge: {
    fontFamily: 'var(--font-mono)',
    fontSize: 10,
    fontWeight: 600,
    padding: '2px 7px',
    borderRadius: 4,
    border: '1px solid',
  },
  pointsTag: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
  },
  joinHint: {
    fontSize: 12,
    color: 'var(--color-text-muted)',
    marginBottom: 28,
  },
  leaderboardList: {
    display: 'flex',
    flexDirection: 'column',
    gap: 6,
    marginBottom: 24,
  },
  leaderboardRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 14,
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: '10px 16px',
  },
  leaderboardRowHighlighted: {
    borderColor: 'var(--color-primary)',
    background: 'rgba(79,70,229,0.08)',
  },
  rank: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    width: 32,
    color: 'var(--color-text-muted)',
  },
  entryUsername: {
    flex: 1,
    fontSize: 14,
    fontWeight: 500,
  },
  entrySolved: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
  },
  entryPoints: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    fontWeight: 600,
    color: 'var(--color-primary)',
    width: 70,
    textAlign: 'right',
  },
};
