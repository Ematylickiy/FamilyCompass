namespace FamilyCompass.Api.Families.Contracts;

public record FamilyMemberResponse(
    Guid UserId,
    string Username,
    string Role
);
