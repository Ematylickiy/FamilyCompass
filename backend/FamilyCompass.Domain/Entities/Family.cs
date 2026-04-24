namespace FamilyCompass.Domain.Entities;

public class Family
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public List<FamilyMembership> Memberships { get; private set; } = [];

    private Family(
        Guid id,
        string name,
        Guid createdByUserId,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
    }

    public static Family Create(string name, Guid createdByUserId)
    {
        return new Family(
            Guid.NewGuid(),
            name,
            createdByUserId,
            DateTime.UtcNow);
    }
}
