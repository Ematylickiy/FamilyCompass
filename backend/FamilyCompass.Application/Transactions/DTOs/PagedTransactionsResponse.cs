namespace FamilyCompass.Application.Transactions.DTOs;

public record PagedTransactionsResponse(
    IReadOnlyList<TransactionResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance
);
