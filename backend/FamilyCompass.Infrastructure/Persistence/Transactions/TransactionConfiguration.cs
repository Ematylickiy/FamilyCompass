using FamilyCompass.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyCompass.Infrastructure.Persistence.Transactions;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Type)
            .IsRequired();

        builder.Property(t => t.Category)
            .IsRequired();

        builder.Property(t => t.Date)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();
        
    }
}
