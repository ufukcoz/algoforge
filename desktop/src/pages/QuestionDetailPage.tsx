import { useEffect, useState } from 'react';
import { getQuestionById, type QuestionDetail } from '../api/client';
import CodeEditorPanel from '../components/CodeEditorPanel';
import AiAssistantPanel from '../components/AiAssistantPanel';

interface QuestionDetailPageProps {
  questionId: string;
  onBack: () => void;
}

export default function QuestionDetailPage({
  questionId,
  onBack,
}: QuestionDetailPageProps) {
  const [question, setQuestion] = useState<QuestionDetail | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [currentCode, setCurrentCode] = useState('');
  const [currentLanguage, setCurrentLanguage] = useState('javascript');

  useEffect(() => {
    setIsLoading(true);
    setError(null);

    getQuestionById(questionId)
      .then(setQuestion)
      .catch((err) => {
        setError(
          err instanceof Error ? err.message : 'Soru yuklenemedi.'
        );
      })
      .finally(() => setIsLoading(false));
  }, [questionId]);

  return (
    <div style={styles.page}>
      <header style={styles.header}>
        <button
          type="button"
          onClick={onBack}
          style={styles.backButton}
        >
          ← Soru listesine don
        </button>
      </header>

      <main style={styles.main}>
        {isLoading && (
          <p style={styles.statusText}>
            yukleniyor...
          </p>
        )}

        {error && (
          <div style={styles.errorBox}>
            {error}
          </div>
        )}

        {question && (
          <div style={styles.workspace}>

            {/* ============================= */}
            {/* SOL TARAF */}
            {/* ============================= */}

            <section style={styles.leftPanel}>

              <div style={styles.questionContent}>

                {/* SORU BAŞLIĞI */}
                <div style={styles.titleRow}>
                  <h1 style={styles.title}>
                    {question.title}
                  </h1>

                  <DifficultyBadge
                    difficulty={question.difficulty}
                  />
                </div>

                {/* META */}
                <div style={styles.metaRow}>
                  <span style={styles.metaTag}>
                    {question.categoryName}
                  </span>

                  <span style={styles.metaTag}>
                    {'{'} {question.timeLimitMs}ms {'}'}
                  </span>

                  <span style={styles.metaTag}>
                    {'{'} {question.memoryLimitMb}MB {'}'}
                  </span>
                </div>

                {/* AÇIKLAMA */}
                <p style={styles.description}>
                  {question.description}
                </p>

                {/* ÖRNEKLER */}
                <h2 style={styles.sectionTitle}>
                  // ornekler
                </h2>

                {question.exampleTestCases.length === 0 && (
                  <p style={styles.statusText}>
                    Bu soru icin ornek test case bulunmuyor.
                  </p>
                )}

                {question.exampleTestCases.map(
                  (testCase, index) => (
                    <div
                      key={index}
                      style={styles.testCaseCard}
                    >
                      <div style={styles.testCaseBlock}>
                        <span style={styles.testCaseLabel}>
                          input
                        </span>

                        <code style={styles.testCaseValue}>
                          {testCase.input}
                        </code>
                      </div>

                      <div style={styles.testCaseBlock}>
                        <span style={styles.testCaseLabel}>
                          output
                        </span>

                        <code style={styles.testCaseValue}>
                          {testCase.expectedOutput}
                        </code>
                      </div>
                    </div>
                  )
                )}

                {/* ============================= */}
                {/* CODE EDITOR + TEST SONUÇLARI */}
                {/* ============================= */}

                <CodeEditorPanel
                  questionId={questionId}
                  onCodeStateChange={(code, language) => {
                    setCurrentCode(code);
                    setCurrentLanguage(language);
                  }}
                />

              </div>

            </section>

            {/* ============================= */}
            {/* SAĞ TARAF - AI */}
            {/* ============================= */}

            <aside style={styles.rightPanel}>

              <AiAssistantPanel
                questionId={questionId}
                code={currentCode}
                language={currentLanguage}
              />

            </aside>

          </div>
        )}
      </main>
    </div>
  );
}


/* ========================================= */
/* DIFFICULTY BADGE */
/* ========================================= */

function DifficultyBadge({
  difficulty,
}: {
  difficulty: string;
}) {
  const colorVar =
    difficulty === 'Easy'
      ? 'var(--color-success)'
      : difficulty === 'Medium'
        ? 'var(--color-warning)'
        : 'var(--color-danger)';

  return (
    <span
      style={{
        ...styles.difficultyBadge,
        color: colorVar,
        borderColor: colorVar,
      }}
    >
      {difficulty}
    </span>
  );
}


/* ========================================= */
/* STYLES */
/* ========================================= */

const styles: Record<string, React.CSSProperties> = {

  page: {
    height: '100%',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden',
  },

  header: {
    padding: '14px 24px',
    borderBottom: '1px solid var(--color-border)',
    background: 'var(--color-bg-elevated)',
    flexShrink: 0,
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
    minHeight: 0,
    width: '100%',
    overflow: 'hidden',
  },

  /* SOL + SAĞ */

  workspace: {
    display: 'grid',
    gridTemplateColumns: 'minmax(0, 1fr) 400px',
    width: '100%',
    height: '100%',
    overflow: 'hidden',
  },

  /* SOL */

  leftPanel: {
    minWidth: 0,
    minHeight: 0,
    overflowY: 'auto',
    overflowX: 'hidden',
    padding: '28px 32px 60px',
    borderRight: '1px solid var(--color-border)',
  },

  questionContent: {
    width: '100%',
    maxWidth: 1000,
    margin: '0 auto',
  },

  /* SAĞ AI */

  rightPanel: {
    width: '400px',
    minWidth: 0,
    minHeight: 0,
    height: '100%',
    overflowY: 'auto',
    overflowX: 'hidden',
    background: 'var(--color-bg-elevated)',
    padding: '20px',
    boxSizing: 'border-box',
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

  titleRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 12,
    marginBottom: 8,
  },

  title: {
    fontSize: 24,
    margin: 0,
  },

  difficultyBadge: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    fontWeight: 600,
    padding: '3px 10px',
    borderRadius: 4,
    border: '1px solid',
  },

  metaRow: {
    display: 'flex',
    gap: 8,
    marginBottom: 20,
    flexWrap: 'wrap',
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

  description: {
    fontSize: 14,
    lineHeight: 1.7,
    color: 'var(--color-text)',
    marginBottom: 28,
    whiteSpace: 'pre-wrap',
  },

  sectionTitle: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    color: 'var(--color-primary)',
    marginBottom: 12,
  },

  testCaseCard: {
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: 14,
    marginBottom: 12,
    display: 'flex',
    flexDirection: 'column',
    gap: 10,
  },

  testCaseBlock: {
    display: 'flex',
    flexDirection: 'column',
    gap: 4,
  },

  testCaseLabel: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
  },

  testCaseValue: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    color: 'var(--color-text)',
    background: 'var(--color-bg)',
    padding: '8px 10px',
    borderRadius: 4,
    display: 'block',
    overflowX: 'auto',
  },
};