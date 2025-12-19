using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure.Persistence.Repositories;

public class BudgetRepository(KontoDbContext dbContext) : IBudgetRepository
{
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

    public async Task AddTransactionAsync(Guid budgetId, Transaction transaction, CancellationToken cancellationToken = default)
    {
        var budget = await dbContext.Budgets.FindAsync(budgetId, cancellationToken);
        if (budget == null)
            throw new InvalidOperationException($"Budget {budgetId} does not exist in the store.");

        await dbContext.Transactions.AddAsync(transaction, cancellationToken);
        dbContext.Entry(transaction).Property("BudgetId").CurrentValue = budgetId;
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

    public async Task DeleteTransactionAsync(Guid budgetId, Guid transactionId, CancellationToken cancellationToken = default)
    {
        var tx = await dbContext.Transactions.FindAsync(transactionId, cancellationToken);
        if (tx == null)
            throw new InvalidOperationException($"Transaction {transactionId} not found");

        dbContext.Transactions.Remove(tx);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}