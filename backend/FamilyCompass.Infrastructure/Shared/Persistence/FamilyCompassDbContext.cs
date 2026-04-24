using FamilyCompass.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyCompass.Infrastructure.Shared.Persistence;

public class FamilyCompassDbContext(DbContextOptions<FamilyCompassDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Family> Families => Set<Family>();
    public DbSet<FamilyMembership> FamilyMemberships => Set<FamilyMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(FamilyCompassDbContext).Assembly
        );
    }
}