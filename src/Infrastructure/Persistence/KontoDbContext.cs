using System.Reflection;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure.Persistence;

public class KontoDbContext : DbContext, IApplicationDbContext
{
    public KontoDbContext(DbContextOptions<KontoDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
