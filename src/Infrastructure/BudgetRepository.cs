using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
namespace KontoApi.Infrastructure;

public class BudgetRepository(KontoDbContext context) : IBudgetRepository
{
    public async Task<Budget?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Budget.FindAsync(userId, cancellationToken);

        if (user == null)
            return null;

        return await BudgetDto(user);
    }

    public async Task AddAsync(Guid budgetId, Transaction transaction, CancellationToken cancellationToken = default)
    {
        var budget = await context.Budget.FirstOrDefaultAsync(b => b.Id == budgetId, cancellationToken);
        if (budget == null)
            throw new NotFoundException($"Budget {budgetId} not found");

        var entity = MapToEntity(budgetId, transaction);
        await context.Transaction.AddAsync(entity, cancellationToken);
        budget.CurrentBalance += transaction.Type == TransactionType.Expense 
            ? -transaction.Amount.Value 
            : transaction.Amount.Value;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(Guid budgetId, IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default)
    {
        var budget = await context.Budget.FirstOrDefaultAsync(b => b.Id == budgetId, cancellationToken);
        if (budget == null)
            throw new NotFoundException($"Budget {budgetId} not found");

        var entities = transactions.Select(t => MapToEntity(budgetId, t)).ToList();
        await context.Transaction.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid budgetId, Guid transactionId, CancellationToken cancellationToken = default)
    {
        var entity = await context.Transaction.FirstOrDefaultAsync(t => 
            t.Id == transactionId && t.BudgetId == budgetId, cancellationToken);
        if (entity == null)
            throw new NotFoundException($"Transaction {transactionId} not found in budget {budgetId}");
        
        context.Transaction.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Budget?> BudgetDto(Models.BudgetEntity budget)
    {
        var dto = new Budget(budget.Name, new Money(budget.CurrentBalance, budget.Currency));
        return dto;
    }

    private static TransactionEntity MapToEntity(Guid budgetId, Transaction transaction)
    {
        return new TransactionEntity
        {
            Id = transaction.Id,
            BudgetId = budgetId,
            TransactionTypeName = transaction.Type.ToString(),
            Amount = transaction.Amount.Value,
            TimeTransaction = transaction.Date,
            Description = transaction.Description,
            CategoryId = transaction.TransactionCategory.Id
        };
    }
}