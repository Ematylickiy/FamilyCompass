import { useState, type SubmitEventHandler } from 'react';
import {
  TransactionType,
  type CreateTransactionRequest,
} from '../api/transactions';

interface Props {
  onSubmit: (data: CreateTransactionRequest) => void;
}

export function TransactionForm({ onSubmit }: Props) {
  const [amount, setAmount] = useState('');
  const [type, setType] = useState<TransactionType>(TransactionType.Income);
  const [category, setCategory] = useState('');
  const [note, setNote] = useState('');

  const handleSubmit: SubmitEventHandler<HTMLFormElement> = (e) => {
    e.preventDefault();
    if (!amount || !category) return;

    onSubmit({
      amount: Number(amount),
      type,
      category,
      date: new Date().toISOString(),
      note: note || undefined,
    });

    setAmount('');
    setCategory('');
    setNote('');
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="number"
        placeholder="Сумма"
        value={amount}
        onChange={(e) => setAmount(e.target.value)}
        required
      />
      <select
        value={type}
        onChange={(e) => setType(e.target.value as TransactionType)}
      >
        <option value="income">Доход</option>
        <option value="expense">Расход</option>
      </select>
      <input
        type="text"
        placeholder="Категория"
        value={category}
        onChange={(e) => setCategory(e.target.value)}
        required
      />
      <input
        type="text"
        placeholder="Заметка (необязательно)"
        value={note}
        onChange={(e) => setNote(e.target.value)}
      />
      <button type="submit">Добавить</button>
    </form>
  );
}
