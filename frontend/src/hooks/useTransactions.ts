import { useCallback, useEffect, useState } from 'react';
import {
  transactionsApi,
  type CreateTransactionRequest,
  type Transaction,
} from '../api/transactions';

let initialTransactionsPromise: Promise<Transaction[]> | null = null;

function loadTransactionsOnce(): Promise<Transaction[]> {
  if (!initialTransactionsPromise) {
    initialTransactionsPromise = transactionsApi.getAll();
  }
  return initialTransactionsPromise;
}

export function useTransactions() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTransactions = useCallback(async () => {
    try {
      setError(null);
      setLoading(true);
      const data = await loadTransactionsOnce();
      setTransactions(data);
    } catch {
      setError('Не удалось загрузить транзакции');
    } finally {
      setLoading(false);
    }
  }, []);

  const addTransaction = useCallback(async (data: CreateTransactionRequest) => {
    try {
      setError(null);
      const created = await transactionsApi.create(data);
      setTransactions((prev) => [created, ...prev]);
    } catch {
      setError('Не удалось создать транзакцию');
    }
  }, []);

  const removeTransaction = useCallback(async (id: string) => {
    try {
      setError(null);
      await transactionsApi.delete(id);
      setTransactions((prev) => prev.filter((t) => t.id !== id));
    } catch {
      setError('Не удалось удалить транзакцию');
    }
  }, []);

  useEffect(() => {
    void fetchTransactions();
  }, [fetchTransactions]);

  return { transactions, loading, error, addTransaction, removeTransaction };
}
