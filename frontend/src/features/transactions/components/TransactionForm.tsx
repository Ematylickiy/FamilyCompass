import { useState, type FormEventHandler } from 'react';
import { TransactionType, type CreateTransactionRequest } from '../types';
import type { FamilyMember } from '../../families/types';
import { Button } from '../../../ui/Button';
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
  members: FamilyMember[];
  initial?: CreateTransactionRequest;
  submitText?: string;
  onCancelEdit?: () => void;
}

const expenseCategories = ['Продукты', 'Транспорт', 'ЖКХ', 'Здоровье', 'Развлечения', 'Другое'];
const incomeCategories = ['Зарплата', 'Подарок', 'Подработка', 'Кэшбек', 'Другое'];

export function TransactionForm({ onSubmit, members, initial, submitText = 'Сохранить', onCancelEdit }: Props) {
  const [amount, setAmount] = useState('');
  const [type, setType] = useState<TransactionType>(initial?.type ?? TransactionType.Expense);
  const [category, setCategory] = useState('');
  const [date, setDate] = useState(() =>
    toDateInputValue(initial?.date ?? new Date().toISOString()),
  );
  const [note, setNote] = useState('');
  const [performedByUserId, setPerformedByUserId] = useState(initial?.performedByUserId ?? members[0]?.userId ?? '');
  const [submitting, setSubmitting] = useState(false);

  const categories = type === TransactionType.Expense ? expenseCategories : incomeCategories;

  const handleSubmit: FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();
    if (submitting || !amount.trim() || !category.trim() || !performedByUserId) return;

    const payload: CreateTransactionRequest = {
      amount: Number(amount),
      type,
      category: category.trim(),
      date: dateInputToIso(date),
      performedByUserId,
      note: note.trim() ? note.trim() : undefined,
    };

    setSubmitting(true);
    await onSubmit(payload);
    setSubmitting(false);

    setAmount('');
    setCategory(categories[0] ?? '');
    setNote('');
    setPerformedByUserId(members[0]?.userId ?? '');
    setDate(toDateInputValue(new Date().toISOString()));
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit} noValidate>
      <input
        className={styles.input}
        type="number"
        min="0.01"
        step="0.01"
        value={amount}
        onChange={(e) => setAmount(e.target.value)}
        placeholder="Сумма"
        required
      />
      <select className={styles.input} value={type} onChange={(e) => setType(e.target.value as TransactionType)}>
        <option value={TransactionType.Expense}>Расход</option>
        <option value={TransactionType.Income}>Доход</option>
      </select>
      <select className={styles.input} value={category} onChange={(e) => setCategory(e.target.value)} required>
        <option value="" disabled>Категория</option>
        {categories.map((item) => (
          <option key={item} value={item}>{item}</option>
        ))}
      </select>
      <input className={styles.input} type="date" value={date} onChange={(e) => setDate(e.target.value)} required />
      <select className={styles.input} value={performedByUserId} onChange={(e) => setPerformedByUserId(e.target.value)} required>
        <option value="" disabled>Кто совершил</option>
        {members.map((member) => (
          <option key={member.userId} value={member.userId}>{member.username}</option>
        ))}
      </select>
      <textarea
        className={styles.input}
        value={note}
        onChange={(e) => setNote(e.target.value)}
        placeholder="Комментарий (необязательно)"
        rows={2}
      />
      <div className={styles.actions}>
        <Button type="submit" variant="brand" disabled={submitting}>
          {submitting ? 'Сохранение...' : submitText}
        </Button>
        {onCancelEdit ? (
          <Button type="button" variant="ghost" onClick={onCancelEdit}>
            Отмена
          </Button>
        ) : null}
      </div>
    </form>
  );
}
