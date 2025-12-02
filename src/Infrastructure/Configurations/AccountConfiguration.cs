using KontoApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KontoApi.Infrastructure.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(account => account.Id);
        builder
            .HasOne(account => account.UserEntity)
            .WithOne()
            .HasForeignKey<AccountEntity>(account => account.UserId)
            .HasPrincipalKey<UserEntity>(user => user.Id);
    }
}