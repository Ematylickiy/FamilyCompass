import './App.css';
import { TransactionForm } from './components/TransactionForm';
import { TransactionList } from './components/TransactionsList';
import { useTransactions } from './hooks/useTransactions';

export default function App() {
  const { transactions, loading, error, addTransaction, removeTransaction } =
    useTransactions();

  if (loading) return <div>Загрузка...</div>;
  if (error) return <div>{error}</div>;

  return (
    <div>
      <h1>Семейные финансы</h1>
      <TransactionForm onSubmit={addTransaction} />
      <TransactionList
        transactions={transactions}
        onDelete={removeTransaction}
      />
    </div>
  );
}
