using KontoApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KontoApi.Infrastructure.Configurations;

public class TransactionTypesConfiguration : IEntityTypeConfiguration<TransactionTypesEntity>
{
    public void Configure(EntityTypeBuilder<TransactionTypesEntity> builder)
    {
        builder.ToTable("TransactionTypes");
        builder.HasKey(transactionType => transactionType.Name);
        builder.Property(type => type.Name).ValueGeneratedNever();
        builder.HasData(
            new TransactionTypesEntity("Income"),
            new TransactionTypesEntity("Expense"),
            new TransactionTypesEntity("Transfer")
        );
    }
}