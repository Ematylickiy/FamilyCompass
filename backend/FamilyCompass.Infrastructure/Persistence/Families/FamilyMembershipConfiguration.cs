using FamilyCompass.Domain.Entities;
using FamilyCompass.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyCompass.Infrastructure.Persistence.Families;

public class FamilyMembershipConfiguration : IEntityTypeConfiguration<FamilyMembership>
{
    public void Configure(EntityTypeBuilder<FamilyMembership> builder)
    {
        builder.HasKey(m => new { m.FamilyId, m.UserId });

        builder.Property(m => m.Role)
            .HasConversion(
                role => role.ToString(),
                value => Enum.Parse<FamilyMembershipRole>(value))
            .IsRequired();

        builder.Property(m => m.JoinedAt)
            .IsRequired();

        builder.HasOne(m => m.Family)
            .WithMany(f => f.Memberships)
            .HasForeignKey(m => m.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User)
            .WithMany(u => u.FamilyMemberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
