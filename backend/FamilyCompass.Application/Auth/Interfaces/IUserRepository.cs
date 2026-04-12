using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task Add(User user, CancellationToken cancellationToken = default);

    Task<User?> GetByUsername(string username, CancellationToken cancellationToken = default);
}