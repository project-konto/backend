using System;
using Microsoft.EntityFrameworkCore;
using KontoApi.Infrastructure.Configurations;
using KontoApi.Infrastructure.Models;

namespace KontoApi.Infrastructure;

public class KontoDbContext : DbContext
{
    public KontoDbContext(DbContextOptions<KontoDbContext> options) : base(options) { }
    public KontoDbContext() { }

    public DbSet<AccountEntity> Account { get; set; }
    public DbSet<BudgetEntity> Budget { get; set; }
    public DbSet<CategoryEntity> Category { get; set; }
    public DbSet<TransactionEntity> Transaction { get; set; }
    public DbSet<UserEntity> User { get; set; }
    public DbSet<TransactionTypesEntity> TransactionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new BudgetConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionTypesConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}