using FamilyCompass.Application.Auth.Interfaces;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyCompass.Infrastructure.Persistence.Users;

public class UsersRepository(FamilyCompassDbContext db): IUserRepository
{
    public async Task Add(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByUsername(string username, CancellationToken cancellationToken = default)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
}