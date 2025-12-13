using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KontoApi.Infrastructure.Persistence.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(b => b.CurrentBalance, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("CurrentBalanceAmount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("CurrentBalanceCurrency")
                .HasMaxLength(3);
        });

        builder.HasMany(b => b.Transactions)
            .WithOne()
            .HasForeignKey("BudgetId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Budget.Transactions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}