import { useState } from 'react';
import Editor from '@monaco-editor/react';

const LANGUAGES = [
  { id: 'javascript', label: 'JavaScript' },
  { id: 'python', label: 'Python' },
  { id: 'cpp', label: 'C++' },
  { id: 'java', label: 'Java' },
  { id: 'csharp', label: 'C#' },
] as const;

const DEFAULT_SNIPPETS: Record<string, string> = {
  javascript: '// cozumunu buraya yaz\nfunction solve(input) {\n  \n}\n',
  python: '# cozumunu buraya yaz\ndef solve(input):\n    pass\n',
  cpp: '// cozumunu buraya yaz\n#include <bits/stdc++.h>\nusing namespace std;\n\nint main() {\n    \n}\n',
  java: '// cozumunu buraya yaz\nclass Solution {\n    public static void main(String[] args) {\n        \n    }\n}\n',
  csharp: '// cozumunu buraya yaz\nclass Solution {\n    static void Main() {\n        \n    }\n}\n',
};

export default function CodeEditorPanel() {
  const [language, setLanguage] = useState<string>('javascript');
  const [codeByLanguage, setCodeByLanguage] = useState<Record<string, string>>({
    ...DEFAULT_SNIPPETS,
  });
  const [runMessage, setRunMessage] = useState<string | null>(null);

  const handleRun = () => {
    // Judge0 entegrasyonu Sprint 4'te gelecek; su an gercekten kod calistirmiyoruz.
    setRunMessage('Judge sistemi henuz hazir degil (Sprint 4). Kodun kaydedildi ama calistirilmadi.');
  };

  return (
    <div style={styles.wrapper}>
      <div style={styles.toolbar}>
        <select
          value={language}
          onChange={(e) => {
            setLanguage(e.target.value);
            setRunMessage(null);
          }}
          style={styles.languageSelect}
        >
          {LANGUAGES.map((lang) => (
            <option key={lang.id} value={lang.id}>
              {lang.label}
            </option>
          ))}
        </select>

        <button type="button" onClick={handleRun} style={styles.runButton}>
          <span style={styles.prompt}>$</span> calistir
        </button>
      </div>

      <div style={styles.editorContainer}>
        <Editor
          height="380px"
          language={language}
          theme="vs-dark"
          value={codeByLanguage[language]}
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

      {runMessage && <div style={styles.runMessage}>{runMessage}</div>}
    </div>
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
  prompt: {
    opacity: 0.8,
  },
  editorContainer: {
    background: '#1e1e1e',
  },
  runMessage: {
    padding: '10px 14px',
    background: 'rgba(245,158,11,0.1)',
    borderTop: '1px solid rgba(245,158,11,0.3)',
    color: '#fcd34d',
    fontSize: 12,
    fontFamily: 'var(--font-mono)',
  },
};
