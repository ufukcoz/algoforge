import { useState } from 'react';
import type { FormEvent } from 'react';
import { login, register } from '../api/client';
import { useAuth } from '../context/AuthContext';

type Mode = 'login' | 'register';

export default function AuthPage() {
  const [mode, setMode] = useState<Mode>('login');
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { setSession } = useAuth();

  const passwordRequirements = {
    minLength: password.length >= 8,
    uppercase: /[A-Z]/.test(password),
    lowercase: /[a-z]/.test(password),
    number: /\d/.test(password),
  };

  const isPasswordStrong =
    passwordRequirements.minLength &&
    passwordRequirements.uppercase &&
    passwordRequirements.lowercase &&
    passwordRequirements.number;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);

    if (mode === 'register' && !isPasswordStrong) {
      setError(
        'Şifre en az 8 karakter, bir büyük harf, bir küçük harf ve bir rakam içermelidir.'
      );
      return;
    }

    setIsSubmitting(true);

    try {
      if (mode === 'register') {
        await register({ username, email, password });

        // Kayıt sonrası otomatik login
        const loginResult = await login({ email, password });

        setSession(
          loginResult.username,
          loginResult.accessToken,
          loginResult.refreshToken
        );
      } else {
        const loginResult = await login({ email, password });

        setSession(
          loginResult.username,
          loginResult.accessToken,
          loginResult.refreshToken
        );
      }
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Bir şeyler ters gitti.'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div style={styles.page}>
      <div style={styles.glow} aria-hidden="true" />

      <div style={styles.brand}>
        <span style={styles.brandMark}>{'</>'}</span>
        <span style={styles.brandName}>AlgoForge</span>
      </div>

      <div style={styles.terminalWindow}>
        <div style={styles.titleBar}>
          <div style={styles.dots}>
            <span
              style={{
                ...styles.dot,
                background: '#EF4444',
              }}
            />

            <span
              style={{
                ...styles.dot,
                background: '#F59E0B',
              }}
            />

            <span
              style={{
                ...styles.dot,
                background: '#22C55E',
              }}
            />
          </div>

          <span style={styles.titleBarLabel}>
            {mode === 'login'
              ? 'auth/login.sh'
              : 'auth/register.sh'}
          </span>
        </div>

        <div style={styles.terminalBody}>
          <p style={styles.comment}>
            {mode === 'login'
              ? '// mevcut hesabınla giriş yap'
              : '// yeni bir hesap oluştur'}
          </p>

          <form onSubmit={handleSubmit} style={styles.form}>
            {mode === 'register' && (
              <Field
                label="username"
                type="text"
                value={username}
                onChange={setUsername}
                placeholder="kullanici_adi"
                required
              />
            )}

            <Field
              label="email"
              type="email"
              value={email}
              onChange={setEmail}
              placeholder="sen@algoforge.com"
              required
            />

            <Field
              label="password"
              type="password"
              value={password}
              onChange={setPassword}
              placeholder="••••••••"
              required
            />

            {mode === 'register' && (
              <div style={styles.passwordRequirements}>
                <span style={styles.requirementTitle}>
                  Şifre gereksinimleri:
                </span>

                <PasswordRequirement
                  valid={passwordRequirements.minLength}
                  text="En az 8 karakter"
                />

                <PasswordRequirement
                  valid={passwordRequirements.uppercase}
                  text="En az 1 büyük harf"
                />

                <PasswordRequirement
                  valid={passwordRequirements.lowercase}
                  text="En az 1 küçük harf"
                />

                <PasswordRequirement
                  valid={passwordRequirements.number}
                  text="En az 1 rakam"
                />
              </div>
            )}

            {error && (
              <div
                role="alert"
                style={styles.errorBox}
              >
                <span style={styles.errorPrompt}>
                  ✕
                </span>

                {error}
              </div>
            )}

            <button
              type="submit"
              disabled={isSubmitting}
              style={styles.submitButton}
            >
              {isSubmitting ? (
                <>Çalıştırılıyor…</>
              ) : (
                <>
                  <span style={styles.prompt}>
                    $
                  </span>{' '}
                  {mode === 'login'
                    ? 'giriş yap'
                    : 'hesap oluştur'}
                </>
              )}
            </button>
          </form>
        </div>
      </div>

      <button
        type="button"
        onClick={() => {
          setError(null);
          setPassword('');
          setMode(
            mode === 'login'
              ? 'register'
              : 'login'
          );
        }}
        style={styles.switchModeButton}
      >
        {mode === 'login'
          ? 'Hesabın yok mu? Kayıt ol →'
          : '← Zaten hesabın var mı? Giriş yap'}
      </button>
    </div>
  );
}

interface FieldProps {
  label: string;
  type: string;
  value: string;
  onChange: (value: string) => void;
  placeholder: string;
  required?: boolean;
}

function Field({
  label,
  type,
  value,
  onChange,
  placeholder,
  required,
}: FieldProps) {
  return (
    <label style={styles.fieldWrapper}>
      <span style={styles.fieldLabel}>
        {label}
      </span>

      <input
        type={type}
        value={value}
        onChange={(e) =>
          onChange(e.target.value)
        }
        placeholder={placeholder}
        required={required}
        style={styles.input}
      />
    </label>
  );
}

interface PasswordRequirementProps {
  valid: boolean;
  text: string;
}

function PasswordRequirement({
  valid,
  text,
}: PasswordRequirementProps) {
  return (
    <span
      style={{
        ...styles.requirement,
        color: valid
          ? '#22C55E'
          : 'var(--color-text-muted)',
      }}
    >
      {valid ? '✓' : '○'} {text}
    </span>
  );
}

const styles: Record<
  string,
  React.CSSProperties
> = {
  page: {
    minHeight: '100%',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    gap: 28,
    padding: '48px 24px',
    position: 'relative',
    overflow: 'hidden',
  },

  glow: {
    position: 'absolute',
    top: '-20%',
    left: '50%',
    transform: 'translateX(-50%)',
    width: 600,
    height: 600,
    background:
      'radial-gradient(circle, rgba(79,70,229,0.18) 0%, rgba(79,70,229,0) 70%)',
    pointerEvents: 'none',
  },

  brand: {
    display: 'flex',
    alignItems: 'center',
    gap: 10,
    fontFamily: 'var(--font-mono)',
    zIndex: 1,
  },

  brandMark: {
    color: 'var(--color-primary)',
    fontSize: 22,
    fontWeight: 700,
  },

  brandName: {
    fontSize: 20,
    fontWeight: 600,
    letterSpacing: '-0.02em',
  },

  terminalWindow: {
    width: '100%',
    maxWidth: 420,
    background: 'var(--color-surface)',
    border: '1px solid var(--color-border)',
    borderRadius: 10,
    overflow: 'hidden',
    boxShadow:
      '0 20px 60px rgba(0,0,0,0.4)',
    zIndex: 1,
  },

  titleBar: {
    display: 'flex',
    alignItems: 'center',
    gap: 12,
    padding: '10px 14px',
    background:
      'var(--color-bg-elevated)',
    borderBottom:
      '1px solid var(--color-border)',
  },

  dots: {
    display: 'flex',
    gap: 6,
  },

  dot: {
    width: 10,
    height: 10,
    borderRadius: '50%',
    display: 'inline-block',
  },

  titleBarLabel: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-text-muted)',
  },

  terminalBody: {
    padding: '28px 24px 24px',
  },

  comment: {
    fontFamily: 'var(--font-mono)',
    fontSize: 13,
    color: 'var(--color-text-muted)',
    marginTop: 0,
    marginBottom: 20,
  },

  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: 16,
  },

  fieldWrapper: {
    display: 'flex',
    flexDirection: 'column',
    gap: 6,
  },

  fieldLabel: {
    fontFamily: 'var(--font-mono)',
    fontSize: 12,
    color: 'var(--color-primary)',
  },

  input: {
    background: 'var(--color-bg)',
    border: '1px solid var(--color-border)',
    borderRadius: 6,
    padding: '10px 12px',
    color: 'var(--color-text)',
    fontFamily: 'var(--font-mono)',
    fontSize: 14,
    outline: 'none',
  },

  passwordRequirements: {
    display: 'flex',
    flexDirection: 'column',
    gap: 5,
    marginTop: -8,
    fontFamily: 'var(--font-mono)',
    fontSize: 11,
  },

  requirementTitle: {
    color: 'var(--color-text-muted)',
    marginBottom: 2,
  },

  requirement: {
    transition: 'color 0.15s ease',
  },

  errorBox: {
    display: 'flex',
    gap: 8,
    alignItems: 'flex-start',
    background: 'rgba(239,68,68,0.1)',
    border:
      '1px solid rgba(239,68,68,0.35)',
    borderRadius: 6,
    padding: '10px 12px',
    fontSize: 13,
    color: '#fca5a5',
  },

  errorPrompt: {
    fontFamily: 'var(--font-mono)',
  },

  submitButton: {
    marginTop: 4,
    background: 'var(--color-primary)',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '11px 16px',
    fontFamily: 'var(--font-mono)',
    fontSize: 14,
    fontWeight: 600,
    cursor: 'pointer',
    transition:
      'background 0.15s ease',
  },

  prompt: {
    opacity: 0.8,
  },

  switchModeButton: {
    background: 'none',
    border: 'none',
    color: 'var(--color-text-muted)',
    fontSize: 13,
    cursor: 'pointer',
    fontFamily: 'var(--font-sans)',
    zIndex: 1,
  },
};