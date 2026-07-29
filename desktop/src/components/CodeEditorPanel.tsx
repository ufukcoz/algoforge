import { useState } from 'react';
import Editor from '@monaco-editor/react';
import { runCode, submitCode, type RunCodeResult, type SubmissionResult } from '../api/client';
import { useAuth } from '../context/AuthContext';

const LANGUAGES = [
  { id: 'javascript', label: 'JavaScript' },
  { id: 'python', label: 'Python' },
  { id: 'cpp', label: 'C++' },
  { id: 'java', label: 'Java' },
  { id: 'csharp', label: 'C#' },
] as const;

const DEFAULT_SNIPPETS: Record<string, string> = {
  javascript:
    '// cozumunu buraya yaz\n// Node.js\'te input() yok, stdin\'i boyle okuyabilirsin:\nconst input = require("fs").readFileSync(0, "utf-8").trim();\n\nfunction solve(input) {\n  \n}\n\nconsole.log(solve(input));\n',
  python: '# cozumunu buraya yaz\ndef solve(input):\n    pass\n',
  cpp: '// cozumunu buraya yaz\n#include <bits/stdc++.h>\nusing namespace std;\n\nint main() {\n    \n}\n',
  java: '// cozumunu buraya yaz (sinif adini istedigin gibi verebilirsin)\nclass Solution {\n    public static void main(String[] args) {\n        \n    }\n}\n',
  csharp: '// cozumunu buraya yaz\nclass Solution {\n    static void Main() {\n        \n    }\n}\n',
};

interface CodeEditorPanelProps {
  questionId: string;
}

export default function CodeEditorPanel({ questionId }: CodeEditorPanelProps) {
  const { accessToken } = useAuth();
  const [language, setLanguage] = useState<string>('javascript');
  const [codeByLanguage, setCodeByLanguage] = useState<Record<string, string>>({
    ...DEFAULT_SNIPPETS,
  });
  const [runResult, setRunResult] = useState<RunCodeResult | null>(null);
  const [submissionResult, setSubmissionResult] = useState<SubmissionResult | null>(null);
  const [isRunning, setIsRunning] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const currentCode = codeByLanguage[language];

  const handleRun = async () => {
    if (!accessToken) return;
    setIsRunning(true);
    setErrorMessage(null);
    setSubmissionResult(null);
    try {
      const result = await runCode(questionId, language, currentCode, accessToken);
      setRunResult(result);
    } catch (err) {
      setErrorMessage(err instanceof Error ? err.message : 'Kod calistirilamadi.');
    } finally {
      setIsRunning(false);
    }
  };

  const handleSubmit = async () => {
    if (!accessToken) return;
    setIsSubmitting(true);
    setErrorMessage(null);
    try {
      const result = await submitCode(questionId, language, currentCode, accessToken);
      setSubmissionResult(result);
    } catch (err) {
      setErrorMessage(err instanceof Error ? err.message : 'Gonderim basarisiz.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div style={styles.wrapper}>
      <div style={styles.toolbar}>
        <select
          value={language}
          onChange={(e) => {
            setLanguage(e.target.value);
            setRunResult(null);
            setSubmissionResult(null);
            setErrorMessage(null);
          }}
          style={styles.languageSelect}
        >
          {LANGUAGES.map((lang) => (
            <option key={lang.id} value={lang.id}>
              {lang.label}
            </option>
          ))}
        </select>

        <div style={styles.toolbarActions}>
          <button type="button" onClick={handleRun} disabled={isRunning} style={styles.runButton}>
            <span style={styles.prompt}>$</span> {isRunning ? 'calisiyor...' : 'calistir'}
          </button>
          <button
            type="button"
            onClick={handleSubmit}
            disabled={isSubmitting}
            style={styles.submitButton}
          >
            {isSubmitting ? 'gonderiliyor...' : 'gonder'}
          </button>
        </div>
      </div>

      <div style={styles.editorContainer}>
        <Editor
          height="340px"
          language={language}
          theme="vs-dark"
          value={currentCode}
          onChange={(value) =>
            setCodeByLanguage((prev) => ({ ...prev, [language]: value ?? '' }))
          }
          options={{
            fontSize: 13,
            fontFamily: "'JetBrains Mono', Consolas, monospace",
            minimap: { enabled: false },
            scrollBeyondLastLine: false,
            automaticLayout: true,
            padding: { top: 12 },
          }}
        />
      </div>

      {errorMessage && <div style={styles.errorBox}>{errorMessage}</div>}

      {runResult && (
        <div style={styles.resultsPanel}>
          <p style={styles.resultsHeader}>
            {'// calistirma sonucu'} {runResult.allPassed ? '— tum ornekler gecti' : '— bazi ornekler basarisiz'}
          </p>
          {runResult.results.map((testCase, index) => (
            <div key={index} style={styles.testCaseResult}>
              <div style={styles.testCaseResultHeader}>
                <span
                  style={{
                    ...styles.statusDot,
                    background: testCase.passed ? 'var(--color-success)' : 'var(--color-danger)',
                  }}
                />
                <span style={styles.testCaseResultLabel}>Ornek {index + 1}</span>
                {testCase.runtimeMs !== null && (
                  <span style={styles.runtimeLabel}>{testCase.runtimeMs}ms</span>
                )}
              </div>
              {!testCase.passed && (
                <div style={styles.diffGrid}>
                  <div>
                    <span style={styles.diffLabel}>beklenen</span>
                    <code style={styles.diffValue}>{testCase.expectedOutput}</code>
                  </div>
                  <div>
                    <span style={styles.diffLabel}>senin ciktin</span>
                    <code style={styles.diffValue}>{testCase.actualOutput ?? '(bos)'}</code>
                  </div>
                </div>
              )}
              {testCase.stderr && <code style={styles.stderrText}>{testCase.stderr}</code>}
              {testCase.compileOutput && (
                <code style={styles.stderrText}>{testCase.compileOutput}</code>
              )}
            </div>
          ))}
        </div>
      )}

      {submissionResult && (
        <div style={styles.submissionPanel}>
          <div style={styles.submissionHeader}>
            <SubmissionStatusBadge status={submissionResult.status} />
            <span style={styles.submissionCounts}>
              {submissionResult.passedCount}/{submissionResult.totalCount} test case gecti
            </span>
          </div>
          {submissionResult.runtimeMs !== null && (
            <span style={styles.runtimeLabel}>
              {submissionResult.runtimeMs}ms · {submissionResult.memoryKb}KB
            </span>
          )}
        </div>
      )}
    </div>
  );
}

function SubmissionStatusBadge({ status }: { status: string }) {
  const colorVar = status === 'Accepted' ? 'var(--color-success)' : 'var(--color-danger)';
  return (
    <span style={{ ...styles.submissionStatusBadge, color: colorVar, borderColor: colorVar }}>
      {status}
    </span>
  );
}

const styles: Record<string, React.CSSProperties> = {
  wrapper: {
    marginTop: 24,
    border: '1px solid var(--color-border)',
    borderRadius: 8,
    overflow: 'hidden',
  },
  toolbar: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '10px 12px',
    background: 'var(--color-bg-elevated)',
    borderBottom: '1px solid var(--color-border)',
  },
  toolbarActions: {
    display: 'flex',
    gap: 8,
  },
  languageSelect: {
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    color: 'var(--color-text)',
    borderRadius: 6,
    padding: '6px 10px',
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
  },
  runButton: {
    background: 'var(--color-primary)',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '7px 14px',
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    fontWeight: 600,
    cursor: 'pointer',
  },
  submitButton: {
    background: 'var(--color-success)',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '7px 14px',
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    fontWeight: 600,
    cursor: 'pointer',
  },
  prompt: {
    opacity: 0.8,
  },
  editorContainer: {
    background: '#1e1e1e',
  },
  errorBox: {
    padding: '10px 14px',
    background: 'rgba(239,68,68,0.1)',
    borderTop: '1px solid rgba(239,68,68,0.3)',
    color: '#fca5a5',
    fontSize: 12,
    fontFamily: 'var(--font-mono)',
  },
  resultsPanel: {
    borderTop: '1px solid var(--color-border)',
    padding: '14px 16px',
    display: 'flex',
    flexDirection: 'column',
    gap: 10,
  },
  resultsHeader: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-text-muted)',
    margin: 0,
  },
  testCaseResult: {
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 6,
    padding: 10,
  },
  testCaseResultHeader: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: '50%',
  },
  testCaseResultLabel: {
    fontSize: 13,
    fontWeight: 500,
  },
  runtimeLabel: {
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
    marginLeft: 'auto',
  },
  diffGrid: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: 10,
    marginTop: 8,
  },
  diffLabel: {
    display: 'block',
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: 'var(--color-text-muted)',
    marginBottom: 4,
  },
  diffValue: {
    display: 'block',
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    background: 'var(--color-bg)',
    padding: '6px 8px',
    borderRadius: 4,
    overflowX: 'auto',
  },
  stderrText: {
    display: 'block',
    marginTop: 8,
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
    color: '#fca5a5',
    whiteSpace: 'pre-wrap',
  },
  submissionPanel: {
    borderTop: '1px solid var(--color-border)',
    padding: '14px 16px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  submissionHeader: {
    display: 'flex',
    alignItems: 'center',
    gap: 10,
  },
  submissionStatusBadge: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    fontWeight: 700,
    padding: '4px 10px',
    borderRadius: 4,
    border: '1px solid',
  },
  submissionCounts: {
    fontSize: 13,
    color: 'var(--color-text-muted)',
  },
};
