using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Category { get; private set; }
    public DateTime Date { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private Transaction(
        Guid id,
        decimal amount,
        TransactionType type,
        string category,
        DateTime date,
        string? note,
        DateTime createdAt
    )
    {
        Id = id;
        Amount = amount;
        Type = type;
        Category = category;
        Date = date;
        Note = note;
        CreatedAt = createdAt;
    }

    public static Transaction Create(
        decimal amount,
        TransactionType type,
        string category,
        DateTime date,
        string? note
    ) =>
        new(
            Guid.NewGuid(),
            amount,
            type,
            category,
            date,
            note,
            DateTime.UtcNow
        );
}