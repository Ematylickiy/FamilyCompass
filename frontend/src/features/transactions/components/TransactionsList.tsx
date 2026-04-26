import { TransactionType, type Transaction } from '../types';
import { Button } from '../../../ui/Button';
import styles from './TransactionsList.module.css';

const money = new Intl.NumberFormat('ru-RU', {
  minimumFractionDigits: 0,
  maximumFractionDigits: 2,
});

const dateTime = new Intl.DateTimeFormat('ru-RU', {
  dateStyle: 'medium',
  timeStyle: 'short',
});

function isIncome(type: TransactionType): boolean {
  return type === TransactionType.Income;
}

interface Props {
  transactions: Transaction[];
  onDelete: (id: string) => void | Promise<void>;
  onEdit: (transaction: Transaction) => void;
}

export function TransactionList({ transactions, onDelete, onEdit }: Props) {
  if (transactions.length === 0) {
    return (
      <p className={styles.empty} role="status">
        Транзакций пока нет — добавьте первую выше.
      </p>
    );
  }

  return (
    <ul className={styles.list}>
      {transactions.map((t) => {
        const income = isIncome(t.type);
        return (
          <li key={t.id} className={styles.card}>
            <div className={styles.header}>
              <span
                className={`${styles.amount} ${income ? styles.amountIncome : styles.amountExpense}`}
              >
                {income ? '+' : '−'}
                {money.format(t.amount)}
              </span>
            </div>
            <div className={styles.meta}>
              <span className={styles.category}>{t.category}</span>
              <span>{dateTime.format(new Date(t.date))}</span>
              <span>{t.performedByUsername}</span>
            </div>
            {t.note ? <p className={styles.note}>{t.note}</p> : null}
            <div className={styles.footer}>
              <Button
                type="button"
                variant="ghost"
                onClick={() => onEdit(t)}
              >
                Редактировать
              </Button>
              <Button
                type="button"
                variant="danger"
                onClick={() => void onDelete(t.id)}
              >
                Удалить
              </Button>
            </div>
          </li>
        );
      })}
    </ul>
  );
}
