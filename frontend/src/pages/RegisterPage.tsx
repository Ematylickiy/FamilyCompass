import { useId, useState, type FormEventHandler } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../api';
import { BrandHeader } from '../features/auth-ui';
import { Alert } from '../ui/Alert';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import styles from './AuthPage.module.css';

export function RegisterPage() {
  const formId = useId();
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit: FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();
    setError(null);
    const u = username.trim();
    if (!u || !password) {
      setError('Enter your email and password');
      return;
    }
    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    setSubmitting(true);
    try {
      await authApi.register(u, password, confirmPassword);
      navigate('/login', {
        replace: true,
        state: {
          flash: 'Account created. Sign in with the same login and password.',
          username: u,
        },
      });
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
          Create account
        </h2>

        <div className={styles.inner}>
          <form className={styles.form} onSubmit={handleSubmit} noValidate>
            {error ? (
              <Alert tone="authError" role="alert">
                {error}
              </Alert>
            ) : null}

            <div className={styles.field}>
              <div className={styles.labelBlock}>
                <label className={styles.label} htmlFor={`${formId}-username`}>
                  Email address
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
              <div className={styles.labelBlock}>
                <label className={styles.label} htmlFor={`${formId}-password`}>
                  Password
                </label>
              </div>
              <div className={styles.inputShell}>
                <input
                  id={`${formId}-password`}
                  className={styles.input}
                  type="password"
                  autoComplete="new-password"
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>
            </div>

            <div className={styles.passwordField}>
              <div className={styles.labelBlock}>
                <label className={styles.label} htmlFor={`${formId}-confirm`}>
                  Confirm password
                </label>
              </div>
              <div className={styles.inputShell}>
                <input
                  id={`${formId}-confirm`}
                  className={styles.input}
                  type="password"
                  autoComplete="new-password"
                  placeholder="••••••••"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                />
              </div>
            </div>

            <Button
              variant="brand"
              type="submit"
              fullWidth
              disabled={submitting}
            >
              {submitting ? 'Creating account…' : 'Create account'}
            </Button>
          </form>
        </div>
      </Card>

      <p className={styles.footer}>
        Already have an account?{' '}
        <Link className={styles.footerLink} to="/login">
          Log in
        </Link>
      </p>
    </div>
  );
}
