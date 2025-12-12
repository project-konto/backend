using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class AccountRepository : IAccountRepository
{
    private readonly KontoDbContext context;

    public AccountRepository(KontoDbContext context)
        => this.context = context;

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await context.Accounts.AddAsync(account, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        context.Accounts.Update(account);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var accountStub = await context.Accounts.FindAsync([id], cancellationToken);

        if (accountStub != null)
        {
            context.Accounts.Remove(accountStub);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Accounts
            .Include(a => a.User)
            .Include(a => a.Budgets)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Accounts
            .Include(a => a.User)
            .Include(a => a.Budgets)
            .Where(a => a.User.Id == userId)
            .ToListAsync(cancellationToken);
    }
}