using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Application.Transactions.DTOs;

public record TransactionResponse(
    Guid Id,
    decimal Amount,
    TransactionType Type,
    string Category,
    DateTime Date,
    string? Note
);
