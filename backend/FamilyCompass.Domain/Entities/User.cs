namespace FamilyCompass.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public string Username { get; init; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; init; }
    public List<FamilyMembership> FamilyMemberships { get; private set; } = [];

    private User (
        Guid id,
        string username,
        string passwordHash,
        DateTime createdAt)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public static User Create(
        string username,
        string passwordHash) => new User(
        Guid.NewGuid(),
        username,
        passwordHash,
        DateTime.UtcNow);
}