import { TransactionForm } from './components/TransactionForm';
import { TransactionList } from './components/TransactionsList';
import { useTransactions } from './hooks/useTransactions';
import styles from './App.module.css';

export default function App() {
  const { transactions, loading, error, addTransaction, removeTransaction } =
    useTransactions();

  if (loading) {
    return (
      <div className={styles.page}>
        <div className={styles.inner}>
          <div className={styles.banner} data-variant="loading" role="status">
            Загрузка…
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <div className={styles.inner}>
        <header className={styles.header}>
          <h1 className={styles.title}>Семейные финансы</h1>
          <p className={styles.subtitle}>
            Учёт доходов и расходов. Данные хранятся на сервере.
          </p>
        </header>

        {error ? (
          <div className={styles.banner} data-variant="error" role="alert">
            {error}
          </div>
        ) : null}

        <section className={styles.panel} aria-labelledby="add-heading">
          <h2 id="add-heading" className={styles.sectionTitle}>
            Новая транзакция
          </h2>
          <TransactionForm onSubmit={addTransaction} />
        </section>

        <section className={styles.panel} aria-labelledby="list-heading">
          <h2 id="list-heading" className={styles.sectionTitle}>
            История
          </h2>
          <TransactionList
            transactions={transactions}
            onDelete={removeTransaction}
          />
        </section>
      </div>
    </div>
  );
}
