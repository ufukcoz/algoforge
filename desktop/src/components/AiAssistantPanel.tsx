import { useState } from 'react';
import {
  getAiAssistance,
  type AiAssistAction,
} from '../api/client';
import { useAuth } from '../context/AuthContext';

interface AiAssistantPanelProps {
  questionId: string;
  code: string;
  language: string;
}

const ACTIONS: {
  id: AiAssistAction;
  label: string;
  icon: string;
}[] = [
  {
    id: 'Hint',
    label: 'Ipucu',
    icon: '💡',
  },
  {
    id: 'ComplexityAnalysis',
    label: 'Karmasiklik',
    icon: '⏱️',
  },
  {
    id: 'ExplainBug',
    label: 'Hata Bul',
    icon: '🐛',
  },
  {
    id: 'ExplainCode',
    label: 'Kodu Acikla',
    icon: '📖',
  },
  {
    id: 'SuggestSolution',
    label: 'Daha Iyi Yaklasim',
    icon: '✨',
  },
];

export default function AiAssistantPanel({
  questionId,
  code,
  language,
}: AiAssistantPanelProps) {
  const { accessToken } = useAuth();

  const [activeAction, setActiveAction] =
    useState<AiAssistAction | null>(null);

  const [response, setResponse] =
    useState<string | null>(null);

  const [isLoading, setIsLoading] =
    useState(false);

  const [error, setError] =
    useState<string | null>(null);

  const handleAskAi = async (
    action: AiAssistAction
  ) => {
    if (!accessToken) return;

    if (!code.trim()) {
      setError(
        'Once editore bir seyler yazmalisin.'
      );
      return;
    }

    setActiveAction(action);
    setIsLoading(true);
    setError(null);
    setResponse(null);

    try {
      const result = await getAiAssistance(
        questionId,
        code,
        language,
        action,
        accessToken
      );

      setResponse(result);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'AI Assistant su an yanit veremiyor.'
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={styles.wrapper}>

      {/* HEADER */}

      <div style={styles.headerRow}>
        <p style={styles.header}>
          {'// AI Assistant'}
        </p>

        <span style={styles.statusDot}>
          ●
        </span>
      </div>

      <p style={styles.description}>
        Kodunu inceleyebilir, hata bulabilir,
        ipucu verebilir ve daha iyi çözüm
        yaklaşımları önerebilirim.
      </p>

      {/* AI BUTONLARI */}

      <div style={styles.actionGrid}>
        {ACTIONS.map((action) => (
          <button
            key={action.id}
            type="button"
            onClick={() =>
              handleAskAi(action.id)
            }
            disabled={isLoading}
            style={{
              ...styles.actionButton,
              ...(activeAction === action.id
                ? styles.actionButtonActive
                : {}),
            }}
          >
            <span style={styles.actionIcon}>
              {action.icon}
            </span>

            <span>
              {action.label}
            </span>
          </button>
        ))}
      </div>

      {/* LOADING */}

      {isLoading && (
        <div style={styles.loadingBox}>
          <span style={styles.loadingIcon}>
            ✨
          </span>

          <span>
            AI dusunuyor...
          </span>
        </div>
      )}

      {/* ERROR */}

      {error && (
        <div style={styles.errorBox}>
          {error}
        </div>
      )}

      {/* RESPONSE */}

      {response && !isLoading && (
        <div style={styles.responseBox}>
          <div style={styles.responseHeader}>
            🤖 AI Assistant
          </div>

          <p style={styles.responseText}>
            {response}
          </p>
        </div>
      )}

      <div style={styles.footer}>
        Dil: {language}
      </div>

    </div>
  );
}

const styles: Record<
  string,
  React.CSSProperties
> = {

  wrapper: {
    width: '100%',
    minHeight: '100%',
    display: 'flex',
    flexDirection: 'column',
    boxSizing: 'border-box',
    border: '1px solid var(--color-border)',
    borderRadius: 10,
    padding: 18,
    background: 'var(--color-surface)',
  },

  headerRow: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: 10,
  },

  header: {
    fontFamily: 'var(--font-mono)',
    fontSize: 14,
    color: 'var(--color-primary)',
    margin: 0,
  },

  statusDot: {
    fontSize: 9,
    color: 'var(--color-success)',
  },

  description: {
    fontSize: 13,
    lineHeight: 1.6,
    color: 'var(--color-text-muted)',
    margin: '0 0 18px',
  },

  actionGrid: {
    display: 'flex',
    flexDirection: 'column',
    gap: 8,
  },

  actionButton: {
    width: '100%',
    display: 'flex',
    alignItems: 'center',
    gap: 10,
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    color: 'var(--color-text)',
    borderRadius: 7,
    padding: '10px 12px',
    fontSize: 12,
    cursor: 'pointer',
    textAlign: 'left',
  },

  actionButtonActive: {
    borderColor: 'var(--color-primary)',
    color: 'var(--color-primary)',
    background: 'var(--color-bg-elevated)',
  },

  actionIcon: {
    width: 20,
    textAlign: 'center',
  },

  loadingBox: {
    marginTop: 16,
    display: 'flex',
    alignItems: 'center',
    gap: 8,
    padding: 12,
    borderRadius: 7,
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-text-muted)',
  },

  loadingIcon: {
    color: 'var(--color-primary)',
  },

  errorBox: {
    marginTop: 16,
    background: 'rgba(239,68,68,0.1)',
    border: '1px solid rgba(239,68,68,0.35)',
    borderRadius: 6,
    padding: '10px 12px',
    fontSize: 13,
    color: '#fca5a5',
  },

  responseBox: {
    marginTop: 18,
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    overflow: 'hidden',
  },

  responseHeader: {
    padding: '10px 12px',
    borderBottom: '1px solid var(--color-border)',
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-primary)',
  },

  responseText: {
    fontSize: 13,
    lineHeight: 1.7,
    color: 'var(--color-text)',
    margin: 0,
    padding: 14,
    whiteSpace: 'pre-wrap',
    overflowWrap: 'anywhere',
  },

  footer: {
    marginTop: 'auto',
    paddingTop: 16,
    color: 'var(--color-text-muted)',
    fontFamily: 'var(--font-mono)',
    fontSize: 10,
  },
};