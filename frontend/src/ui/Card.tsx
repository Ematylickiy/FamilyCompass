import type { ComponentPropsWithRef, ElementType, ReactNode } from 'react';
import styles from './Card.module.css';

export type CardVariant = 'auth' | 'glass';

type CardOwnProps<T extends ElementType> = {
  variant: CardVariant;
  hero?: boolean;
  as?: T;
  children: ReactNode;
  className?: string;
};

export function Card<T extends ElementType = 'div'>({
  variant,
  hero = false,
  as,
  children,
  className,
  ...rest
}: CardOwnProps<T> & Omit<ComponentPropsWithRef<T>, keyof CardOwnProps<T> | 'ref'>) {
  const Component = (as ?? 'div') as ElementType;
  const surface =
    variant === 'auth'
      ? styles.auth
      : [styles.surface, hero ? styles.hero : ''].filter(Boolean).join(' ');

  return (
    <Component className={[surface, className ?? ''].filter(Boolean).join(' ')} {...rest}>
      {children}
    </Component>
  );
}
