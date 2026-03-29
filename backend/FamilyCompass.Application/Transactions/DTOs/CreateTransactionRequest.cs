namespace FamilyCompass.Application.Transactions.DTOs;

public record CreateTransactionRequest(
    decimal Amount,
    string Type,
    string Category,
    DateTime Date,
    string? Note
);