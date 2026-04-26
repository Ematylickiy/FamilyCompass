using FamilyCompass.Application.Families.DTOs;

namespace FamilyCompass.Application.Families.Interfaces;

public interface IFamilyService
{
    Task<FamilyResponse> CreateAsync(
        CreateFamilyRequest request,
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    Task<List<FamilyResponse>> GetMineAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid familyId,
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    Task<List<FamilyMemberResponse>> GetMembersAsync(
        Guid familyId,
        Guid currentUserId,
        CancellationToken cancellationToken = default);
}
