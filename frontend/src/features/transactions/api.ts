import client from '../../api/http/client';
import type {
  CreateTransactionRequest,
  PagedTransactionsResponse,
  TransactionsQuery,
  Transaction,
  UpdateTransactionRequest,
} from './types';

export const transactionsApi = {
  getByFamily: (familyId: string, query: TransactionsQuery = {}) =>
    client.get<PagedTransactionsResponse>(`families/${familyId}/transactions`, {
      params: {
        page: query.page,
        pageSize: query.pageSize,
        from: query.from,
        to: query.to,
        type: query.type,
        category: query.category,
        performedByUserId: query.performedByUserId,
      },
    }),
  create: (familyId: string, data: CreateTransactionRequest) =>
    client.post<Transaction>(`families/${familyId}/transactions`, data),
  update: (familyId: string, transactionId: string, data: UpdateTransactionRequest) =>
    client.put<Transaction>(`families/${familyId}/transactions/${transactionId}`, data),
  delete: (familyId: string, transactionId: string) =>
    client.delete(`families/${familyId}/transactions/${transactionId}`),
};
