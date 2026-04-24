import type { ButtonHTMLAttributes, ReactNode } from 'react';
import styles from './Button.module.css';

export type ButtonVariant = 'brand' | 'accent' | 'danger' | 'ghost' | 'social';

type Props = {
  variant: ButtonVariant;
  fullWidth?: boolean;
  className?: string;
  children: ReactNode;
} & Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'className'>;

const variantClass: Record<ButtonVariant, string> = {
  brand: styles.brand,
  accent: styles.accent,
  danger: styles.danger,
  ghost: styles.ghost,
  social: styles.social,
};

export function Button({
  variant,
  fullWidth,
  className,
  children,
  type = 'button',
  ...rest
}: Props) {
  return (
    <button
      type={type}
      className={[
        styles.root,
        variantClass[variant],
        fullWidth ? styles.fullWidth : '',
        className ?? '',
      ]
        .filter(Boolean)
        .join(' ')}
      {...rest}
    >
      {children}
    </button>
  );
}
