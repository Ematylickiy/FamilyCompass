using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Domain.Entities;

public class FamilyMembership
{
    public Guid FamilyId { get; private set; }
    public Guid UserId { get; private set; }
    public FamilyMembershipRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Family Family { get; private set; }
    public User User { get; private set; }

    private FamilyMembership(
        Guid familyId,
        Guid userId,
        FamilyMembershipRole role,
        DateTime joinedAt)
    {
        FamilyId = familyId;
        UserId = userId;
        Role = role;
        JoinedAt = joinedAt;
    }

    public static FamilyMembership CreateOwner(Guid familyId, Guid userId)
    {
        return new FamilyMembership(
            familyId,
            userId,
            FamilyMembershipRole.Owner,
            DateTime.UtcNow);
    }
}
