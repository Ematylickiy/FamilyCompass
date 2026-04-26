using FamilyCompass.Application.Families.Interfaces;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyCompass.Infrastructure.Persistence.Families;

public class FamilyMembershipRepository(FamilyCompassDbContext db) : IFamilyMembershipRepository
{
    public async Task AddAsync(FamilyMembership membership, CancellationToken cancellationToken = default)
    {
        await db.FamilyMemberships.AddAsync(membership, cancellationToken);
    }

    public Task<FamilyMembership?> GetByFamilyAndUserIdAsync(
        Guid familyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return db.FamilyMemberships
            .FirstOrDefaultAsync(
                m => m.FamilyId == familyId && m.UserId == userId,
                cancellationToken);
    }

    public Task<List<FamilyMembership>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken = default)
    {
        return db.FamilyMemberships
            .Include(m => m.User)
            .Where(m => m.FamilyId == familyId)
            .OrderBy(m => m.JoinedAt)
            .ToListAsync(cancellationToken);
    }
}
