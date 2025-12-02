using KontoApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KontoApi.Infrastructure.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.HasKey(transaction => transaction.Id);
        builder
            .HasOne(transaction => transaction.CategoryEntity)
            .WithOne()
            .HasForeignKey<TransactionEntity>(category => category.CategoryId)
            .HasPrincipalKey<CategoryEntity>(user => user.Id);

        builder
            .HasOne(transaction => transaction.BudgetEntity)
            .WithOne()
            .HasForeignKey<TransactionEntity>(budget => budget.BudgetId)
            .HasPrincipalKey<BudgetEntity>(user => user.Id);

        builder
            .HasOne(type => type.TransactionTypesEntity)
            .WithMany()
            .HasForeignKey(type => type.TransactionTypeName)
            .IsRequired();
    }
}