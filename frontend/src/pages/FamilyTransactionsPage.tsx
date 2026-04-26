import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import styles from '../App.module.css';
import { Alert } from '../ui/Alert';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import { familiesApi } from '../features/families/api';
import type { FamilyMember } from '../features/families/types';
import {
  TransactionForm,
  TransactionList,
  useTransactions,
  type CreateTransactionRequest,
  type Transaction,
} from '../features/transactions';

export function FamilyTransactionsPage() {
  const navigate = useNavigate();
  const { familyId = '' } = useParams();
  const [members, setMembers] = useState<FamilyMember[]>([]);
  const [membersLoading, setMembersLoading] = useState(true);
  const [editing, setEditing] = useState<Transaction | null>(null);
  const { transactions, totals, loading, error, addTransaction, updateTransaction, removeTransaction } = useTransactions(familyId);

  useEffect(() => {
    if (!familyId) return;
    void (async () => {
      try {
        setMembersLoading(true);
        const data = await familiesApi.getMembers(familyId);
        setMembers(data);
      } finally {
        setMembersLoading(false);
      }
    })();
  }, [familyId]);

  const handleSubmit = async (payload: CreateTransactionRequest) => {
    if (editing) {
      await updateTransaction(editing.id, payload);
      setEditing(null);
      return;
    }

    await addTransaction(payload);
  };

  if (!familyId) return <Alert tone="error">Семья не найдена</Alert>;

  return (
    <div className={styles.page}>
      <div className={styles.inner}>
        <Card variant="glass" hero as="header" className={styles.header}>
          <div className={styles.headerRow}>
            <h1 className={styles.title}>Транзакции семьи</h1>
            <Button type="button" variant="ghost" onClick={() => navigate('/')}>
              Назад
            </Button>
          </div>
        </Card>

        {error ? (
          <Alert tone="error" role="alert" className={styles.banner}>
            {error}
          </Alert>
        ) : null}

        <Card variant="glass" as="section">
          <h2 className={styles.sectionTitle}>{editing ? 'Редактирование транзакции' : 'Новая транзакция'}</h2>
          {membersLoading ? (
            <Alert tone="loading">Загрузка участников...</Alert>
          ) : (
            <TransactionForm
              members={members}
              initial={
                editing
                  ? {
                      amount: editing.amount,
                      type: editing.type,
                      category: editing.category,
                      date: editing.date,
                      performedByUserId: editing.performedByUserId,
                      note: editing.note ?? undefined,
                    }
                  : undefined
              }
              submitText={editing ? 'Сохранить' : 'Добавить'}
              onCancelEdit={() => setEditing(null)}
              onSubmit={handleSubmit}
            />
          )}
        </Card>

        <Card variant="glass" as="section">
          <h2 className={styles.sectionTitle}>
            История ({transactions.length}) | Доход: {totals.income} | Расход: {totals.expense} | Баланс: {totals.balance}
          </h2>
          {loading ? (
            <Alert tone="loading">Загрузка транзакций...</Alert>
          ) : (
            <TransactionList
              transactions={transactions}
              onDelete={(id) => removeTransaction(id)}
              onEdit={(transaction) => setEditing(transaction)}
            />
          )}
        </Card>
      </div>
    </div>
  );
}
