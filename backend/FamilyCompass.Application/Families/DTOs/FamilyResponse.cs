using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Application.Families.DTOs;

public sealed record FamilyResponse(
    Guid Id,
    string Name,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    FamilyMembershipRole Role);
