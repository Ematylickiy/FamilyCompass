using FamilyCompass.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyCompass.Infrastructure.Shared.Persistence;

public class FamilyCompassDbContext(DbContextOptions<FamilyCompassDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(FamilyCompassDbContext).Assembly
        );
    }
}