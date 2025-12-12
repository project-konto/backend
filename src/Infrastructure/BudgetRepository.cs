using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class BudgetRepository : IBudgetRepository
{
    private readonly KontoDbContext context;

    public BudgetRepository(KontoDbContext context)
        => this.context = context;

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Budgets
            .Include(b => b.Transactions)
            .Include(b => b.Transactions).ThenInclude(t => t.TransactionCategory)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddAsync(Budget budget, CancellationToken cancellationToken)
    {
        await context.Budgets.AddAsync(budget, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Budget budget, CancellationToken cancellationToken)
    {
        context.Budgets.Update(budget);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var budgetStub = await context.Budgets.FindAsync([id], cancellationToken);

        if (budgetStub != null)
        {
            context.Budgets.Remove(budgetStub);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}