import { TransactionType, type Transaction } from '../api/transactions';

interface Props {
  transactions: Transaction[];
  onDelete: (id: string) => void;
}

export function TransactionList({ transactions, onDelete }: Props) {
  if (transactions.length === 0) {
    return <p>Транзакций пока нет</p>;
  }

  return (
    <ul>
      {transactions.map((t) => (
        <li key={t.id}>
          <span>
            {t.type === TransactionType.Income ? '+' : '-'}
            {t.amount}
          </span>
          <p>
            Категория:
            <span>{t.category}</span>
          </p>
          <span>{new Date(t.date).toLocaleDateString('ru-RU')}</span>
          <p>{t.note && <span>{t.note}</span>}</p>
          <div>
            <button onClick={() => onDelete(t.id)}>Удалить</button>
          </div>
        </li>
      ))}
    </ul>
  );
}
