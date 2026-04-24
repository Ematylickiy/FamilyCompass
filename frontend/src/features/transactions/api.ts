import client from '../../api/http/client';
import type { CreateTransactionRequest, Transaction } from './types';

export const transactionsApi = {
  getAll: () => client.get<Transaction[]>('transactions'),
  create: (data: CreateTransactionRequest) =>
    client.post<Transaction>('transactions', data),
  delete: (id: string) => client.delete(`transactions/${id}`),
};
