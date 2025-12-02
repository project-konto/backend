using KontoApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KontoApi.Infrastructure.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<BudgetEntity>
{
    public void Configure(EntityTypeBuilder<BudgetEntity> builder)
    {
        builder.HasKey(budget => budget.Id);
        builder
            .HasOne(budget => budget.AccountEntity)
            .WithOne()
            .HasForeignKey<BudgetEntity>(account => account.AccountId)
            .HasPrincipalKey<AccountEntity>(user => user.Id);
    }
}