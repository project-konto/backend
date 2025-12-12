using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class BudgetRepository : IBudgetRepository
{
    private readonly KontoDbContext dbContext;

    public BudgetRepository(KontoDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Budgets
            .Include(b => b.Transactions)
            .Include(b => b.Transactions).ThenInclude(t => t.TransactionCategory)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddAsync(Budget budget, CancellationToken cancellationToken)
    {
        await dbContext.Budgets.AddAsync(budget, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Budget budget, CancellationToken cancellationToken)
    {
        dbContext.Budgets.Update(budget);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var budgetStub = await dbContext.Budgets.FindAsync([id], cancellationToken);

        if (budgetStub != null)
        {
            dbContext.Budgets.Remove(budgetStub);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}