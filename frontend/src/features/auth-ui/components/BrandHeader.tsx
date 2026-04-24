import styles from './BrandHeader.module.css';

type BrandHeaderProps = {
  subtitle: string;
};

export function BrandHeader({ subtitle }: BrandHeaderProps) {
  return (
    <header className={styles.header}>
      <div className={styles.logoRow}>
        <div className={styles.logoMark} aria-hidden />
        <div className={styles.titleWrap}>
          <h1 className={styles.brandTitle}>Family Compass</h1>
        </div>
      </div>
      <p className={styles.tagline}>{subtitle}</p>
    </header>
  );
}
