import { useState } from 'react';
import { getAiAssistance, type AiAssistAction } from '../api/client';
import { useAuth } from '../context/AuthContext';

interface AiAssistantPanelProps {
  questionId: string;
  code: string;
  language: string;
}

const ACTIONS: { id: AiAssistAction; label: string; icon: string }[] = [
  { id: 'Hint', label: 'Ipucu', icon: '\ud83d\udca1' },
  { id: 'ComplexityAnalysis', label: 'Karmasiklik', icon: '\u23f1\ufe0f' },
  { id: 'ExplainBug', label: 'Hata Bul', icon: '\ud83d\udc1b' },
  { id: 'ExplainCode', label: 'Kodu Acikla', icon: '\ud83d\udcd6' },
  { id: 'SuggestSolution', label: 'Daha Iyi Yaklasim', icon: '\u2728' },
];

export default function AiAssistantPanel({ questionId, code, language }: AiAssistantPanelProps) {
  const { accessToken } = useAuth();
  const [activeAction, setActiveAction] = useState<AiAssistAction | null>(null);
  const [response, setResponse] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleAskAi = async (action: AiAssistAction) => {
    if (!accessToken) return;

    if (!code.trim()) {
      setError('Once editore bir seyler yazmalisin.');
      return;
    }

    setActiveAction(action);
    setIsLoading(true);
    setError(null);
    setResponse(null);

    try {
      const result = await getAiAssistance(questionId, code, language, action, accessToken);
      setResponse(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'AI Assistant su an yanit veremiyor.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={styles.wrapper}>
      <p style={styles.header}>{'// AI Assistant'}</p>

      <div style={styles.actionGrid}>
        {ACTIONS.map((action) => (
          <button
            key={action.id}
            type="button"
            onClick={() => handleAskAi(action.id)}
            disabled={isLoading}
            style={{
              ...styles.actionButton,
              ...(activeAction === action.id ? styles.actionButtonActive : {}),
            }}
          >
            <span>{action.icon}</span>
            <span>{action.label}</span>
          </button>
        ))}
      </div>

      {isLoading && <p style={styles.statusText}>AI dusunuyor...</p>}
      {error && <div style={styles.errorBox}>{error}</div>}
      {response && !isLoading && (
        <div style={styles.responseBox}>
          <p style={styles.responseText}>{response}</p>
        </div>
      )}
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  wrapper: {
    marginTop: 20,
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: 16,
    background: 'var(--color-surface)',
  },
  header: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-primary)',
    margin: '0 0 12px',
  },
  actionGrid: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: 8,
  },
  actionButton: {
    display: 'flex',
    alignItems: 'center',
    gap: 6,
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    color: 'var(--color-text)',
    borderRadius: 6,
    padding: '7px 12px',
    fontSize: 12,
    cursor: 'pointer',
  },
  actionButtonActive: {
    borderColor: 'var(--color-primary)',
    color: 'var(--color-primary)',
  },
  statusText: {
    marginTop: 12,
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-text-muted)',
  },
  errorBox: {
    marginTop: 12,
    background: 'rgba(239,68,68,0.1)',
    border: '1px solid rgba(239,68,68,0.35)',
    borderRadius: 6,
    padding: '10px 12px',
    fontSize: 13,
    color: '#fca5a5',
  },
  responseBox: {
    marginTop: 12,
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    borderRadius: 6,
    padding: '12px 14px',
  },
  responseText: {
    fontSize: 13,
    lineHeight: 1.7,
    color: 'var(--color-text)',
    margin: 0,
    whiteSpace: 'pre-wrap',
  },
};
