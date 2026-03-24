import { useId, useState, type FormEventHandler } from 'react';
import {
  TransactionType,
  type CreateTransactionRequest,
} from '../api/transactions';
import styles from './TransactionForm.module.css';

function toDateInputValue(iso: string): string {
  return iso.slice(0, 10);
}

function dateInputToIso(date: string): string {
  const d = new Date(`${date}T12:00:00`);
  return Number.isNaN(d.getTime()) ? new Date().toISOString() : d.toISOString();
}

interface Props {
  onSubmit: (data: CreateTransactionRequest) => void | Promise<void>;
}

export function TransactionForm({ onSubmit }: Props) {
  const formId = useId();
  const [amount, setAmount] = useState('');
  const [type, setType] = useState<TransactionType>(TransactionType.Income);
  const [category, setCategory] = useState('');
  const [date, setDate] = useState(() => toDateInputValue(new Date().toISOString()));
  const [note, setNote] = useState('');

  const handleSubmit: FormEventHandler<HTMLFormElement> = (e) => {
    e.preventDefault();
    if (!amount.trim() || !category.trim()) return;

    const payload: CreateTransactionRequest = {
      amount: Number(amount),
      type,
      category: category.trim(),
      date: dateInputToIso(date),
      note: note.trim() ? note.trim() : undefined,
    };

    void onSubmit(payload);

    setAmount('');
    setCategory('');
    setNote('');
    setDate(toDateInputValue(new Date().toISOString()));
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit} noValidate>
      <div className={styles.row}>
        <label className={styles.label} htmlFor={`${formId}-amount`}>
          Сумма
        </label>
        <input
          id={`${formId}-amount`}
          className={styles.input}
          type="number"
          inputMode="decimal"
          min="0"
          step="0.01"
          placeholder="0.00"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
          required
          autoComplete="transaction-amount"
        />
      </div>

      <div className={styles.row}>
        <label className={styles.label} htmlFor={`${formId}-type`}>
          Тип
        </label>
        <select
          id={`${formId}-type`}
          className={styles.select}
          value={type}
          onChange={(e) => setType(e.target.value as TransactionType)}
        >
          <option value={TransactionType.Income}>Доход</option>
          <option value={TransactionType.Expense}>Расход</option>
        </select>
      </div>

      <div className={styles.row}>
        <label className={styles.label} htmlFor={`${formId}-date`}>
          Дата
        </label>
        <input
          id={`${formId}-date`}
          className={styles.input}
          type="date"
          value={date}
          onChange={(e) => setDate(e.target.value)}
          required
        />
      </div>

      <div className={styles.row}>
        <label className={styles.label} htmlFor={`${formId}-category`}>
          Категория
        </label>
        <input
          id={`${formId}-category`}
          className={styles.input}
          type="text"
          placeholder="Например, продукты"
          value={category}
          onChange={(e) => setCategory(e.target.value)}
          required
          autoComplete="off"
        />
      </div>

      <div className={styles.row}>
        <label className={styles.label} htmlFor={`${formId}-note`}>
          Заметка <span className={styles.optional}>(необязательно)</span>
        </label>
        <input
          id={`${formId}-note`}
          className={styles.input}
          type="text"
          placeholder="Комментарий"
          value={note}
          onChange={(e) => setNote(e.target.value)}
        />
      </div>

      <div className={styles.actions}>
        <button className={styles.submit} type="submit">
          Добавить транзакцию
        </button>
      </div>
    </form>
  );
}
