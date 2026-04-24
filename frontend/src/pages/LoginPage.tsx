import { useEffect, useId, useState, type FormEventHandler } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { authApi } from '../api';
import { useAuth } from '../auth/useAuth';
import { BrandHeader } from '../features/auth-ui';
import appleIcon from '../assets/apple-392106.png';
import googleIcon from '../assets/google.png';
import { Alert } from '../ui/Alert';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import styles from './LoginPage.module.css';

type LoginLocationState = {
  flash?: string;
  username?: string;
  from?: string;
};

export function LoginPage() {
  const formId = useId();
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);

  useEffect(() => {
    const state = (location.state ?? null) as LoginLocationState | null;
    if (!state) return;
    if (state.flash) setInfo(state.flash);
    if (state.username) setUsername(state.username);
    if (state.flash || state.username) {
      const nextState = state.from ? { from: state.from } : {};
      navigate(location.pathname, { replace: true, state: nextState });
    }
  }, [location.pathname, location.state, navigate]);

  const handleSubmit: FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();
    setError(null);
    const u = username.trim();
    if (!u || !password) {
      setError('Enter your email and password');
      return;
    }

    setSubmitting(true);
    try {
      const { accessToken } = await authApi.login(u, password);
      login(accessToken);
      const state = (location.state ?? null) as LoginLocationState | null;
      const from = state?.from;
      const target =
        from && from !== '/login' && from.startsWith('/') ? from : '/';
      navigate(target, { replace: true });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className={styles.page}>
      <BrandHeader subtitle="Спокойствие за финансы начинается здесь" />

      <Card variant="auth" as="section" aria-labelledby={`${formId}-title`}>
        <h2 id={`${formId}-title`} className="visually-hidden">
          Войти
        </h2>

        <div className={styles.inner}>
          <form className={styles.form} onSubmit={handleSubmit} noValidate>
            {info ? (
              <Alert tone="hint" role="status">
                {info}
              </Alert>
            ) : null}
            {error ? (
              <Alert tone="authError" role="alert">
                {error}
              </Alert>
            ) : null}

            <div className={styles.field}>
              <div className={styles.labelBlock}>
                <label className={styles.label} htmlFor={`${formId}-username`}>
                  Email
                </label>
              </div>
              <div className={styles.inputShell}>
                <input
                  id={`${formId}-username`}
                  className={styles.input}
                  type="text"
                  autoComplete="username"
                  placeholder="name@family.com"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  required
                />
              </div>
            </div>

            <div className={styles.passwordField}>
              <div className={styles.passwordHeader}>
                <label className={styles.label} htmlFor={`${formId}-password`}>
                  Пароль
                </label>
                <button
                  type="button"
                  className={styles.forgotLink}
                  onClick={() => {}}
                >
                  забыли пароль?
                </button>
              </div>
              <div className={styles.inputShell}>
                <input
                  id={`${formId}-password`}
                  className={styles.input}
                  type="password"
                  autoComplete="current-password"
                  placeholder="........"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>
            </div>

            <Button
              variant="brand"
              type="submit"
              fullWidth
              className={styles.submitButton}
              disabled={submitting}
            >
              {submitting ? 'Вход...' : 'Войти'}
            </Button>

            <p className={styles.continueLabel}>Продолжить с помощью</p>

            <div className={styles.socialRow}>
              <button type="button" className={styles.socialButton} disabled>
                <img src={appleIcon} alt="" aria-hidden className={styles.socialIcon} />
                Apple
              </button>
              <button type="button" className={styles.socialButton} disabled>
                <img src={googleIcon} alt="" aria-hidden className={styles.socialIcon} />
                Google
              </button>
            </div>
          </form>
        </div>
      </Card>

      <p className={styles.footer}>
        Еще нет аккаунта?{' '}
        <Link className={styles.footerLink} to="/register">
          Создать новый аккаунт
        </Link>
      </p>
    </div>
  );
}
