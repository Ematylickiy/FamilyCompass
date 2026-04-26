using FamilyCompass.Application.Families.Interfaces;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyCompass.Infrastructure.Persistence.Families;

public class FamilyRepository(FamilyCompassDbContext db) : IFamilyRepository
{
    public async Task AddAsync(Family family, CancellationToken cancellationToken = default)
    {
        await db.Families.AddAsync(family, cancellationToken);
    }

    public Task<List<Family>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return db.Families
            .Include(f => f.Memberships)
            .Where(f => f.Memberships.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public Task<Family?> GetByIdWithMembershipsAsync(Guid familyId, CancellationToken cancellationToken = default)
    {
        return db.Families
            .Include(f => f.Memberships)
            .FirstOrDefaultAsync(f => f.Id == familyId, cancellationToken);
    }

    public void Remove(Family family)
    {
        db.Families.Remove(family);
    }
}
