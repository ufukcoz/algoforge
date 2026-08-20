import { useEffect, useState } from 'react';
import { getProfile, type ProfileData } from '../api/client';
import { useAuth } from '../context/AuthContext';

export default function ProfilePage() {
  const { accessToken } = useAuth();
  const [profile, setProfile] = useState<ProfileData | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!accessToken) return;
    setIsLoading(true);
    getProfile(accessToken)
      .then(setProfile)
      .catch((err) => setError(err instanceof Error ? err.message : 'Profil yuklenemedi.'))
      .finally(() => setIsLoading(false));
  }, [accessToken]);

  if (isLoading) return <div style={styles.page}><p style={styles.statusText}>yukleniyor...</p></div>;
  if (error) return <div style={styles.page}><div style={styles.errorBox}>{error}</div></div>;
  if (!profile) return null;

  // Basit seviye ilerleme cubugu: bir sonraki seviyeye kadar 1000 XP gerekiyor (User.AddXp ile ayni kural).
  const xpIntoLevel = profile.xp % 1000;
  const progressPercent = (xpIntoLevel / 1000) * 100;

  return (
    <div style={styles.page}>
      <div style={styles.headerCard}>
        <p style={styles.eyebrow}>{'// kullanici profili'}</p>
        <h1 style={styles.username}>{profile.username}</h1>
        <p style={styles.email}>{profile.email}</p>

        <div style={styles.levelRow}>
          <span style={styles.levelBadge}>Level {profile.level}</span>
          <div style={styles.progressTrack}>
            <div style={{ ...styles.progressFill, width: `${progressPercent}%` }} />
          </div>
          <span style={styles.xpText}>{xpIntoLevel} / 1000 XP</span>
        </div>
      </div>

      <div style={styles.statsGrid}>
        <StatCard label="toplam XP" value={profile.xp} />
        <StatCard label="cozulen soru" value={profile.questionsSolved} />
        <StatCard label="toplam gonderim" value={profile.totalSubmissions} />
        <StatCard label="kabul edilen" value={profile.acceptedSubmissions} />
      </div>

      <div style={styles.metaCard}>
        <MetaRow label="uye olma tarihi" value={new Date(profile.memberSince).toLocaleDateString('tr-TR')} />
      </div>
    </div>
  );
}

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <div style={styles.statCard}>
      <span style={styles.statValue}>{value}</span>
      <span style={styles.statLabel}>{label}</span>
    </div>
  );
}

function MetaRow({ label, value }: { label: string; value: string }) {
  return (
    <div style={styles.metaRow}>
      <span style={styles.metaLabel}>{label}</span>
      <span style={styles.metaValue}>{value}</span>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    padding: '28px 32px',
    maxWidth: 640,
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
  headerCard: {
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 10,
    padding: 24,
    marginBottom: 20,
  },
  eyebrow: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-success)',
    margin: 0,
  },
  username: {
    fontSize: 24,
    margin: '8px 0 4px',
  },
  email: {
    fontSize: 13,
    color: 'var(--color-text-muted)',
    marginBottom: 20,
  },
  levelRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 12,
  },
  levelBadge: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    fontWeight: 700,
    color: 'var(--color-primary)',
    border: '1px solid var(--color-primary)',
    borderRadius: 4,
    padding: '4px 10px',
    whiteSpace: 'nowrap',
  },
  progressTrack: {
    flex: 1,
    height: 8,
    background: 'var(--color-bg)',
    borderRadius: 4,
    overflow: 'hidden',
  },
  progressFill: {
    height: '100%',
    background: 'var(--color-primary)',
    borderRadius: 4,
    transition: 'width 0.3s ease',
  },
  xpText: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
    whiteSpace: 'nowrap',
  },
  statsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(4, 1fr)',
    gap: 12,
    marginBottom: 20,
  },
  statCard: {
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: '16px 12px',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    gap: 4,
  },
  statValue: {
    fontSize: 22,
    fontWeight: 700,
    fontFamily: 'var(--font-mono)',
    color: 'var(--color-primary)',
  },
  statLabel: {
    fontSize: 11,
    color: 'var(--color-text-muted)',
    textAlign: 'center',
  },
  metaCard: {
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: 16,
    display: 'flex',
    flexDirection: 'column',
    gap: 10,
  },
  metaRow: {
    display: 'flex',
    justifyContent: 'space-between',
    fontSize: 13,
  },
  metaLabel: {
    color: 'var(--color-text-muted)',
  },
  metaValue: {
    color: 'var(--color-text)',
  },
};
