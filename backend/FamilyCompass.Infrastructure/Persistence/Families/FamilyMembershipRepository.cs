using FamilyCompass.Application.Families.Interfaces;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Infrastructure.Shared.Persistence;

namespace FamilyCompass.Infrastructure.Persistence.Families;

public class FamilyMembershipRepository(FamilyCompassDbContext db) : IFamilyMembershipRepository
{
    public async Task AddAsync(FamilyMembership membership, CancellationToken cancellationToken = default)
    {
        await db.FamilyMemberships.AddAsync(membership, cancellationToken);
    }
}
