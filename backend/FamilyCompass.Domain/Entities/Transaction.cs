namespace FamilyCompass.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction()
    {
    }

    private Transaction(
        Guid id,
        decimal amount,
        string type,
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
        string type,
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