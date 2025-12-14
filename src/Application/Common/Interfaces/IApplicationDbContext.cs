using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
