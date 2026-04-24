using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Api.Families.Contracts;

public sealed record CreateFamilyResponse(
    Guid Id,
    string Name,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    FamilyMembershipRole Role);
