using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Families.Interfaces;

public interface IFamilyMembershipRepository
{
    Task AddAsync(FamilyMembership membership, CancellationToken cancellationToken = default);
    Task<FamilyMembership?> GetByFamilyAndUserIdAsync(Guid familyId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<FamilyMembership>> GetByFamilyIdAsync(Guid familyId, CancellationToken cancellationToken = default);
}
