using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid FamilyId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Category { get; private set; }
    public DateTime Date { get; private set; }
    public Guid PerformedByUserId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    
    private Transaction(
        Guid id,
        Guid familyId,
        decimal amount,
        TransactionType type,
        string category,
        DateTime date,
        Guid performedByUserId,
        Guid createdByUserId,
        string? note,
        DateTime createdAt,
        DateTime updatedAt,
        Guid? updatedByUserId
    )
    {
        Id = id;
        FamilyId = familyId;
        Amount = amount;
        Type = type;
        Category = category;
        Date = date;
        PerformedByUserId = performedByUserId;
        CreatedByUserId = createdByUserId;
        Note = note;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UpdatedByUserId = updatedByUserId;
    }

    public static Transaction Create(
        Guid familyId,
        decimal amount,
        TransactionType type,
        string category,
        DateTime date,
        Guid performedByUserId,
        Guid createdByUserId,
        string? note
    )
    {
        ValidateIdentifiers(familyId, performedByUserId, createdByUserId);
        ValidateAmount(amount);

        var normalizedCategory = NormalizeCategory(category);
        var normalizedDate = NormalizeDate(date);
        var normalizedNote = NormalizeNote(note);
        var now = DateTime.UtcNow;

        return new Transaction(
            Guid.NewGuid(),
            familyId,
            amount,
            type,
            normalizedCategory,
            normalizedDate,
            performedByUserId,
            createdByUserId,
            normalizedNote,
            now,
            now,
            null);
    }

    public static Transaction Create(
        decimal amount,
        TransactionType type,
        string category,
        DateTime date,
        string? note)
        => Create(
            Guid.NewGuid(),
            amount,
            type,
            category,
            date,
            Guid.NewGuid(),
            Guid.NewGuid(),
            note);

    public void Update(
        decimal amount,
        TransactionType type,
        string category,
        DateTime date,
        Guid performedByUserId,
        Guid updatedByUserId,
        string? note)
    {
        ValidateAmount(amount);

        if (performedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Performed by user id is required.", nameof(performedByUserId));
        }

        if (updatedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Updated by user id is required.", nameof(updatedByUserId));
        }

        Amount = amount;
        Type = type;
        Category = NormalizeCategory(category);
        Date = NormalizeDate(date);
        PerformedByUserId = performedByUserId;
        Note = NormalizeNote(note);
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;
    }

    private static void ValidateIdentifiers(Guid familyId, Guid performedByUserId, Guid createdByUserId)
    {
        if (familyId == Guid.Empty)
        {
            throw new ArgumentException("Family id is required.", nameof(familyId));
        }

        if (performedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Performed by user id is required.", nameof(performedByUserId));
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("Created by user id is required.", nameof(createdByUserId));
        }
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }
    }

    private static string NormalizeCategory(string category)
    {
        var normalizedCategory = category?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCategory))
        {
            throw new ArgumentException("Category is required.", nameof(category));
        }

        return normalizedCategory;
    }

    private static DateTime NormalizeDate(DateTime date)
    {
        if (date == default)
        {
            throw new ArgumentException("Date is required.", nameof(date));
        }

        return date;
    }

    private static string? NormalizeNote(string? note)
    {
        var normalizedNote = note?.Trim();
        return string.IsNullOrWhiteSpace(normalizedNote) ? null : normalizedNote;
    }
}