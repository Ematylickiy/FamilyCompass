namespace FamilyCompass.Application.Families.DTOs;

public record FamilyMemberResponse(
    Guid UserId,
    string Username,
    string Role
);
