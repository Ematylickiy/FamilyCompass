using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Families.Interfaces;

public interface IFamilyRepository
{
    Task AddAsync(Family family, CancellationToken cancellationToken = default);
    Task<List<Family>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
