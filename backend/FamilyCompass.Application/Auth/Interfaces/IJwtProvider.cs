using FamilyCompass.Application.Auth.DTOs;

namespace FamilyCompass.Application.Auth.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(JwtTokenSubject subject);
}