import { useCallback, useEffect, useState } from 'react';
import { transactionsApi } from '../api';
import type {
  CreateTransactionRequest,
  PagedTransactionsResponse,
  Transaction,
  UpdateTransactionRequest,
} from '../types';

export function useTransactions(familyId: string) {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [totals, setTotals] = useState({
    income: 0,
    expense: 0,
    balance: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTransactions = useCallback(async (): Promise<PagedTransactionsResponse | null> => {
    if (!familyId) return null;

    try {
      setError(null);
      setLoading(true);
      const data = await transactionsApi.getByFamily(familyId, { page: 1, pageSize: 50 });
      setTransactions(data.items);
      setTotals({
        income: data.totalIncome,
        expense: data.totalExpense,
        balance: data.balance,
      });
      return data;
    } catch {
      setError('Не удалось загрузить транзакции');
      return null;
    } finally {
      setLoading(false);
    }
  }, [familyId]);

  const addTransaction = useCallback(async (data: CreateTransactionRequest) => {
    try {
      setError(null);
      await transactionsApi.create(familyId, data);
      await fetchTransactions();
    } catch {
      setError('Не удалось создать транзакцию');
    }
  }, [familyId, fetchTransactions]);

  const updateTransaction = useCallback(async (transactionId: string, data: UpdateTransactionRequest) => {
    try {
      setError(null);
      await transactionsApi.update(familyId, transactionId, data);
      await fetchTransactions();
    } catch {
      setError('Не удалось обновить транзакцию');
    }
  }, [familyId, fetchTransactions]);

  const removeTransaction = useCallback(async (id: string) => {
    try {
      setError(null);
      await transactionsApi.delete(familyId, id);
      setTransactions((prev) => prev.filter((t) => t.id !== id));
    } catch {
      setError('Не удалось удалить транзакцию');
    }
  }, [familyId]);

  useEffect(() => {
    void fetchTransactions();
  }, [fetchTransactions]);

  return { transactions, totals, loading, error, addTransaction, updateTransaction, removeTransaction, reload: fetchTransactions };
}
