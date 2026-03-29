namespace FamilyCompass.Application.Transactions.DTOs;

public record TransactionResponse(
    Guid Id,
    decimal Amount,
    string Type,
    string Category,
    DateTime Date,
    string? Note,
    DateTime CreatedAt
);
