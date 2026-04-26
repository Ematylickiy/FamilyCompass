import { useEffect, useRef, type ReactNode } from 'react';
import { Button } from './Button';
import styles from './Modal.module.css';

type Props = {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
};

export function Modal({ open, onClose, title, children }: Props) {
  const ref = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const d = ref.current;
    if (!d) return;
    if (open) {
      if (!d.open) d.showModal();
    } else if (d.open) {
      d.close();
    }
  }, [open]);

  return (
    <dialog
      ref={ref}
      className={styles.dialog}
      aria-labelledby="modal-title"
      onClick={(e) => {
        if (e.target === e.currentTarget) e.currentTarget.close();
      }}
      onClose={onClose}
    >
      <div className={styles.panel} onClick={(e) => e.stopPropagation()}>
        <header className={styles.header}>
          <h2 id="modal-title" className={styles.title}>
            {title}
          </h2>
          <Button
            type="button"
            variant="ghost"
            className={styles.closeBtn}
            onClick={() => ref.current?.close()}
            aria-label="Закрыть"
          >
            ×
          </Button>
        </header>
        <div className={styles.body}>{children}</div>
      </div>
    </dialog>
  );
}
