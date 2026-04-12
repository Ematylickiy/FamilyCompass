namespace FamilyCompass.Application.Auth.DTOs;

public sealed record JwtTokenSubject(Guid UserId, string Username);
