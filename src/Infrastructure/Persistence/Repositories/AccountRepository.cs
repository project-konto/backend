using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure.Repositories;

public class AccountRepository(KontoDbContext dbContext) : IAccountRepository
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        // Load the tracked entity and apply changes to avoid concurrency issues with InMemory provider
        var existing = await dbContext.Accounts
            .Include(a => a.Budgets)
            .FirstOrDefaultAsync(a => a.Id == account.Id, cancellationToken);

        if (existing == null)
            throw new InvalidOperationException($"Account {account.Id} does not exist in the store.");

        // Update scalar properties
        existing.Rename(account.Name);

        // Sync budgets: add new, remove missing
        var incomingBudgetIds = account.Budgets.Select(b => b.Id).ToHashSet();
        var existingBudgetIds = existing.Budgets.Select(b => b.Id).ToList();

        // Remove budgets that were removed
        foreach (var eb in existingBudgetIds)
        {
            if (!incomingBudgetIds.Contains(eb))
            {
                existing.RemoveBudget(eb);
            }
        }

        // Add budgets that are new
        foreach (var budget in account.Budgets)
        {
            if (!existing.Budgets.Any(b => b.Id == budget.Id))
            {
                existing.AddBudget(budget);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var accountStub = await dbContext.Accounts.FindAsync(id, cancellationToken);

        if (accountStub != null)
        {
            dbContext.Accounts.Remove(accountStub);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task AddBudgetAsync(Guid accountId, Budget budget, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts.FindAsync(accountId, cancellationToken);
        if (account == null)
            throw new InvalidOperationException($"Account {accountId} does not exist in the store.");

        await dbContext.Budgets.AddAsync(budget, cancellationToken);
        // set shadow FK AccountId
        dbContext.Entry(budget).Property("AccountId").CurrentValue = accountId;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Accounts
            .Include(a => a.User)
            .Include(a => a.Budgets)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await dbContext.Accounts
            .Include(a => a.User)
            .Include(a => a.Budgets)
            .Where(a => a.User.Id == userId)
            .ToListAsync(cancellationToken);
}