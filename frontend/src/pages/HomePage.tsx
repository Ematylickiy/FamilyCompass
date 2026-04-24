import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/useAuth';
import {
  TransactionForm,
  TransactionList,
  useTransactions,
} from '../features/transactions';
import { Alert } from '../ui/Alert';
import { Button } from '../ui/Button';
import { Card } from '../ui/Card';
import styles from '../App.module.css';

export function HomePage() {
  const navigate = useNavigate();
  const { logout } = useAuth();
  const { transactions, loading, error, addTransaction, removeTransaction } =
    useTransactions();

  const handleLogout = () => {
    logout();
    navigate('/login', { replace: true });
  };

  if (loading) {
    return (
      <div className={styles.page}>
        <div className={styles.inner}>
          <Alert tone="loading" role="status" className={styles.banner}>
            Загрузка…
          </Alert>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <div className={styles.inner}>
        <Card variant="glass" hero as="header" className={styles.header}>
          <div className={styles.headerRow}>
            <h1 className={styles.title}>Семейные финансы</h1>
            <Button type="button" variant="ghost" onClick={handleLogout}>
              Выйти
            </Button>
          </div>
        </Card>

        {error ? (
          <Alert tone="error" role="alert" className={styles.banner}>
            {error}
          </Alert>
        ) : null}

        <Card variant="glass" as="section" aria-labelledby="add-heading">
          <h2 id="add-heading" className={styles.sectionTitle}>
            Новая транзакция
          </h2>
          <TransactionForm onSubmit={addTransaction} />
        </Card>

        <Card variant="glass" as="section" aria-labelledby="list-heading">
          <h2 id="list-heading" className={styles.sectionTitle}>
            История
          </h2>
          <TransactionList
            transactions={transactions}
            onDelete={removeTransaction}
          />
        </Card>
      </div>
    </div>
  );
}
