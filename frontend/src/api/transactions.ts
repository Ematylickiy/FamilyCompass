import client from './client';

export const TransactionType = {
  Income: 'income',
  Expense: 'Expense',
} as const;

export type TransactionType =
  (typeof TransactionType)[keyof typeof TransactionType];

export interface Transaction {
  id: string;
  amount: number;
  type: TransactionType;
  category: string;
  date: string;
  note?: string;
}

export type CreateTransactionRequest = Omit<Transaction, 'id'>;

export const transactionsApi = {
  getAll: () => client.get<Transaction[]>('transactions'),
  create: (data: CreateTransactionRequest) =>
    client.post<Transaction>('transactions', data),
  delete: (id: string) => client.delete(`transactions/${id}`),
};
