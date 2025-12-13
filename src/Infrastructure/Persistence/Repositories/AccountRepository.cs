using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly KontoDbContext dbContext;

    public AccountRepository(KontoDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        dbContext.Accounts.Update(account);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var accountStub = await dbContext.Accounts.FindAsync([id], cancellationToken);

        if (accountStub != null)
        {
            dbContext.Accounts.Remove(accountStub);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
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