export const TransactionType = {
  Income: 'income',
  Expense: 'expense',
} as const;

export type TransactionType =
  (typeof TransactionType)[keyof typeof TransactionType];

export interface Transaction {
  id: string;
  familyId: string;
  amount: number;
  type: TransactionType;
  category: string;
  date: string;
  performedByUserId: string;
  performedByUsername: string;
  note?: string | null;
}

export interface CreateTransactionRequest {
  amount: number;
  type: TransactionType;
  category: string;
  date: string;
  performedByUserId: string;
  note?: string;
}

export type UpdateTransactionRequest = CreateTransactionRequest;

export interface TransactionsQuery {
  page?: number;
  pageSize?: number;
  from?: string;
  to?: string;
  type?: TransactionType;
  category?: string;
  performedByUserId?: string;
}

export interface PagedTransactionsResponse {
  items: Transaction[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalIncome: number;
  totalExpense: number;
  balance: number;
}
