import type { HTMLAttributes, ReactNode } from 'react';
import styles from './Alert.module.css';

export type AlertTone = 'loading' | 'error' | 'hint' | 'authError';

type Props = {
  tone: AlertTone;
  children: ReactNode;
  className?: string;
} & Omit<HTMLAttributes<HTMLDivElement>, 'className'>;

const toneClass: Record<AlertTone, string> = {
  loading: styles.loading,
  error: styles.error,
  hint: styles.hint,
  authError: styles.authError,
};

export function Alert({ tone, children, className, ...rest }: Props) {
  return (
    <div
      className={[toneClass[tone], className ?? ''].filter(Boolean).join(' ')}
      {...rest}
    >
      {children}
    </div>
  );
}
