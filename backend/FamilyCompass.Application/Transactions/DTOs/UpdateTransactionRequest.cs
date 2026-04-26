using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Application.Transactions.DTOs;

public record UpdateTransactionRequest(
    decimal Amount,
    TransactionType Type,
    string Category,
    DateTime Date,
    Guid PerformedByUserId,
    string? Note
);
