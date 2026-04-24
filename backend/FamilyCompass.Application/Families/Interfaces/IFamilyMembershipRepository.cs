using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Families.Interfaces;

public interface IFamilyMembershipRepository
{
    Task AddAsync(FamilyMembership membership, CancellationToken cancellationToken = default);
}
