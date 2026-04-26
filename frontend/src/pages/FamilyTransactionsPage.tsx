import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import styles from '../App.module.css';
import pageStyles from './FamilyTransactionsPage.module.css';
import { Alert } from '../ui/Alert';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import { Modal } from '../ui/Modal';
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
  const [formOpen, setFormOpen] = useState(false);
  const { transactions, loading, error, addTransaction, updateTransaction, removeTransaction } =
    useTransactions(familyId);

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

  const closeForm = () => {
    setFormOpen(false);
    setEditing(null);
  };

  const handleSubmit = async (payload: CreateTransactionRequest) => {
    if (editing) {
      await updateTransaction(editing.id, payload);
    } else {
      await addTransaction(payload);
    }
    closeForm();
  };

  if (!familyId) return <Alert tone="error">Семья не найдена</Alert>;

  return (
    <div className={`${styles.page} ${pageStyles.page}`}>
      <div className={`${styles.inner} ${pageStyles.inner}`}>
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
          <div className={pageStyles.toolbar}>
            <h2 className={styles.sectionTitle}>Список</h2>
            <Button
              type="button"
              variant="brand"
              onClick={() => {
                setEditing(null);
                setFormOpen(true);
              }}
            >
              Добавить транзакцию
            </Button>
          </div>
          {loading ? (
            <Alert tone="loading">Загрузка транзакций...</Alert>
          ) : (
            <TransactionList
              transactions={transactions}
              onDelete={(id) => removeTransaction(id)}
              onEdit={(transaction) => {
                setEditing(transaction);
                setFormOpen(true);
              }}
            />
          )}
        </Card>
      </div>

      <Modal
        open={formOpen}
        onClose={closeForm}
        title={editing ? 'Редактирование' : 'Новая транзакция'}
      >
        {membersLoading ? (
          <Alert tone="loading">Загрузка участников...</Alert>
        ) : (
          <TransactionForm
            key={editing?.id ?? 'new'}
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
            onCancel={closeForm}
            onSubmit={handleSubmit}
          />
        )}
      </Modal>
    </div>
  );
}
