using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Application.Transactions.DTOs;

public record CreateTransactionRequest(
    decimal Amount,
    TransactionType Type,
    string Category,
    DateTime Date,
    string? Note
);