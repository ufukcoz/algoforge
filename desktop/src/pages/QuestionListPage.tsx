import { useEffect, useState } from 'react';
import {
  getCategories,
  getQuestions,
  type Category,
  type QuestionSummary,
} from '../api/client';
import { useAuth } from '../context/AuthContext';

const DIFFICULTIES = ['Easy', 'Medium', 'Hard'] as const;

interface QuestionListPageProps {
  onSelectQuestion: (id: string) => void;
}

export default function QuestionListPage({ onSelectQuestion }: QuestionListPageProps) {
  const { username, logout } = useAuth();
  const [categories, setCategories] = useState<Category[]>([]);
  const [questions, setQuestions] = useState<QuestionSummary[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);
  const [selectedDifficulty, setSelectedDifficulty] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getCategories()
      .then(setCategories)
      .catch(() => {
        // Kategori yuklenemese bile soru listesi filtresiz calismaya devam eder.
      });
  }, []);

  useEffect(() => {
    setIsLoading(true);
    setError(null);
    getQuestions({
      categoryId: selectedCategoryId ?? undefined,
      difficulty: selectedDifficulty ?? undefined,
    })
      .then((result) => {
        setQuestions(result.items);
        setTotalCount(result.totalCount);
      })
      .catch((err) => {
        setError(err instanceof Error ? err.message : 'Sorular yuklenemedi.');
      })
      .finally(() => setIsLoading(false));
  }, [selectedCategoryId, selectedDifficulty]);

  return (
    <div style={styles.page}>
      <header style={styles.header}>
        <div style={styles.brand}>
          <span style={styles.brandMark}>{'</>'}</span>
          <span style={styles.brandName}>AlgoForge</span>
        </div>
        <div style={styles.headerRight}>
          <span style={styles.username}>{username}</span>
          <button type="button" onClick={logout} style={styles.logoutButton}>
            Cikis yap
          </button>
        </div>
      </header>

      <div style={styles.body}>
        <aside style={styles.sidebar}>
          <p style={styles.sidebarLabel}>// kategori</p>
          <button
            type="button"
            onClick={() => setSelectedCategoryId(null)}
            style={{
              ...styles.filterItem,
              ...(selectedCategoryId === null ? styles.filterItemActive : {}),
            }}
          >
            Tumu
          </button>
          {categories.map((category) => (
            <button
              key={category.id}
              type="button"
              onClick={() => setSelectedCategoryId(category.id)}
              style={{
                ...styles.filterItem,
                ...(selectedCategoryId === category.id ? styles.filterItemActive : {}),
              }}
            >
              {category.name}
              <span style={styles.filterCount}>{category.questionCount}</span>
            </button>
          ))}

          <p style={{ ...styles.sidebarLabel, marginTop: 24 }}>// zorluk</p>
          <button
            type="button"
            onClick={() => setSelectedDifficulty(null)}
            style={{
              ...styles.filterItem,
              ...(selectedDifficulty === null ? styles.filterItemActive : {}),
            }}
          >
            Tumu
          </button>
          {DIFFICULTIES.map((difficulty) => (
            <button
              key={difficulty}
              type="button"
              onClick={() => setSelectedDifficulty(difficulty)}
              style={{
                ...styles.filterItem,
                ...(selectedDifficulty === difficulty ? styles.filterItemActive : {}),
              }}
            >
              {difficulty}
            </button>
          ))}
        </aside>

        <main style={styles.main}>
          <p style={styles.resultCount}>
            {isLoading ? 'yukleniyor...' : `${totalCount} soru bulundu`}
          </p>

          {error && <div style={styles.errorBox}>{error}</div>}

          {!isLoading && !error && questions.length === 0 && (
            <div style={styles.emptyState}>
              Bu filtrelerle eslesen soru yok. Henuz backend'e soru eklenmemis olabilir.
            </div>
          )}

          <div style={styles.list}>
            {questions.map((question) => (
              <button
                key={question.id}
                type="button"
                onClick={() => onSelectQuestion(question.id)}
                style={styles.questionRow}
              >
                <span style={styles.questionTitle}>{question.title}</span>
                <span style={styles.questionMeta}>
                  <span style={styles.categoryTag}>{question.categoryName}</span>
                  <DifficultyBadge difficulty={question.difficulty} />
                </span>
              </button>
            ))}
          </div>
        </main>
      </div>
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
  body: {
    flex: 1,
    display: 'flex',
    minHeight: 0,
  },
  sidebar: {
    width: 200,
    borderRight: '1px solid var(--color-border)',
    padding: '20px 12px',
    overflowY: 'auto',
  },
  sidebarLabel: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-text-muted)',
    padding: '0 8px',
    marginBottom: 8,
  },
  filterItem: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    width: '100%',
    background: 'none',
    border: 'none',
    color: 'var(--color-text)',
    fontSize: 13,
    padding: '7px 8px',
    borderRadius: 6,
    cursor: 'pointer',
    textAlign: 'left',
  },
  filterItemActive: {
    background: 'rgba(79,70,229,0.15)',
    color: 'var(--color-primary)',
    fontWeight: 600,
  },
  filterCount: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
  },
  main: {
    flex: 1,
    padding: '20px 28px',
    overflowY: 'auto',
  },
  resultCount: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    color: 'var(--color-text-muted)',
    marginTop: 0,
    marginBottom: 16,
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
  emptyState: {
    color: 'var(--color-text-muted)',
    fontSize: 14,
    padding: '32px 0',
    textAlign: 'center',
  },
  list: {
    display: 'flex',
    flexDirection: 'column',
    gap: 8,
  },
  questionRow: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    padding: '14px 16px',
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
    gap: 10,
  },
  categoryTag: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
    background: 'var(--color-bg)',
    padding: '3px 8px',
    borderRadius: 4,
  },
  difficultyBadge: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    fontWeight: 600,
    padding: '3px 8px',
    borderRadius: 4,
    border: '1px solid',
  },
};
