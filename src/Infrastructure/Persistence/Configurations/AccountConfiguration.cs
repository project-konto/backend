using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KontoApi.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<Account>("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Budgets)
            .WithOne()
            .HasForeignKey("AccountId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Account.Budgets))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}