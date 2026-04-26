using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Application.Transactions.DTOs;

public record TransactionsQuery(
    DateTime? From,
    DateTime? To,
    TransactionType? Type,
    string? Category,
    Guid? PerformedByUserId,
    int Page = 1,
    int PageSize = 20
);
