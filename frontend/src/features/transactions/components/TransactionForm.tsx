import { useId, useState, type FormEventHandler } from 'react';
import { TransactionType, type CreateTransactionRequest } from '../types';
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
  const [date, setDate] = useState(() =>
    toDateInputValue(new Date().toISOString()),
  );
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
    <form className={styles.form} onSubmit={handleSubmit} noValidate></form>
  );
}
