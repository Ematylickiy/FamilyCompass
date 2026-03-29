import client from './client';

export const TransactionType = {
  Income: 'income',
  Expense: 'expense',
} as const;

export type TransactionType =
  (typeof TransactionType)[keyof typeof TransactionType];

export interface Transaction {
  id: string;
  amount: number;
  type: TransactionType;
  category: string;
  date: string;
  note?: string | null;
}

export type CreateTransactionRequest = Omit<Transaction, 'id' | 'createdAt'>;

export const transactionsApi = {
  getAll: () => client.get<Transaction[]>('transactions'),
  create: (data: CreateTransactionRequest) =>
    client.post<Transaction>('transactions', data),
  delete: (id: string) => client.delete(`transactions/${id}`),
};
