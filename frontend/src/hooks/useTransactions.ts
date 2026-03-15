import { useState, useEffect } from 'react';
import {
  transactionsApi,
  type CreateTransactionRequest,
  type Transaction,
} from '../api/transactions';

export function useTransactions() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const data = await transactionsApi.getAll();
      setTransactions(data);
    } catch {
      setError('Не удалось загрузить транзакции');
    } finally {
      setLoading(false);
    }
  };

  const addTransaction = async (data: CreateTransactionRequest) => {
    try {
      const created = await transactionsApi.create(data);
      setTransactions((prev) => [created, ...prev]);
    } catch {
      setError('Не удалось создать транзакцию');
    }
  };

  const removeTransaction = async (id: string) => {
    try {
      debugger;
      await transactionsApi.delete(id);
      setTransactions((prev) => prev.filter((t) => t.id !== id));
    } catch (e) {
      setError('Не удалось удалить транзакцию');
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, []);

  return { transactions, loading, error, addTransaction, removeTransaction };
}
