using FamilyCompass.Application.Persistence;

namespace FamilyCompass.Infrastructure.Shared.Persistence;

public sealed class EfUnitOfWork(FamilyCompassDbContext db) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
