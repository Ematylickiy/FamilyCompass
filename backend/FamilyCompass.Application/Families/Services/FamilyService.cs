using FamilyCompass.Application.Families.DTOs;
using FamilyCompass.Application.Families.Exceptions;
using FamilyCompass.Application.Families.Interfaces;
using FamilyCompass.Application.Persistence;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Application.Families.Services;

public class FamilyService(
    IFamilyRepository familyRepository,
    IFamilyMembershipRepository membershipRepository,
    IUnitOfWork unitOfWork) : IFamilyService
{
    public async Task<FamilyResponse> CreateAsync(
        CreateFamilyRequest request,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Family name is required.");

        if (name.Length > 128)
            throw new ArgumentException("Family name is too long. Maximum is 128 characters.");

        var family = Family.Create(name, currentUserId);
        var ownerMembership = FamilyMembership.CreateOwner(family.Id, currentUserId);

        await familyRepository.AddAsync(family, cancellationToken);
        await membershipRepository.AddAsync(ownerMembership, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new FamilyResponse(
            family.Id,
            family.Name,
            family.CreatedByUserId,
            family.CreatedAt,
            ownerMembership.Role);
    }

    public async Task<List<FamilyResponse>> GetMineAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var families = await familyRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        return families
            .OrderByDescending(f => f.CreatedAt)
            .Select(f =>
            {
                var membership = f.Memberships.First(m => m.UserId == currentUserId);
                return new FamilyResponse(
                    f.Id,
                    f.Name,
                    f.CreatedByUserId,
                    f.CreatedAt,
                    membership.Role);
            })
            .ToList();
    }

    public async Task DeleteAsync(
        Guid familyId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var family = await familyRepository.GetByIdWithMembershipsAsync(familyId, cancellationToken);
        if (family is null)
            throw new FamilyNotFoundException(familyId);

        var membership = family.Memberships.FirstOrDefault(m => m.UserId == currentUserId);
        if (membership is null || membership.Role != FamilyMembershipRole.Owner)
            throw new InsufficientFamilyPermissionsException();

        familyRepository.Remove(family);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<FamilyMemberResponse>> GetMembersAsync(
        Guid familyId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var family = await familyRepository.GetByIdWithMembershipsAsync(familyId, cancellationToken);
        if (family is null)
            throw new FamilyNotFoundException(familyId);

        var currentMembership = family.Memberships.FirstOrDefault(m => m.UserId == currentUserId);
        if (currentMembership is null)
            throw new InsufficientFamilyPermissionsException();

        return family.Memberships
            .OrderBy(m => m.JoinedAt)
            .Select(m => new FamilyMemberResponse(
                m.UserId,
                m.User.Username,
                m.Role.ToString()))
            .ToList();
    }
}
