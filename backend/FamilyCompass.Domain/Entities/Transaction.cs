namespace FamilyCompass.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
};