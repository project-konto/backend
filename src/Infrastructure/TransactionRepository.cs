using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace KontoApi.Infrastructure;

public class TransactionRepository(KontoDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var transactionEntity =
            new TransactionEntity
            {
                Id = transaction.Id,
                TransactionTypeName = transaction.TransactionCategory.Name,
                Amount = transaction.Amount.Value,
                CategoryId = transaction.TransactionCategory.Id,
                TimeTransaction = transaction.Date,
                Description = transaction.Description
            };

        await context.Transaction.AddAsync(transactionEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default)
    {
        foreach (var transaction in transactions)
        {
            var transactionEntity = new TransactionEntity
            {
                Id = transaction.Id,
                TransactionTypeName = transaction.TransactionCategory.Name,
                Amount = transaction.Amount.Value,
                CategoryId = transaction.TransactionCategory.Id,
                TimeTransaction = transaction.Date,
                Description = transaction.Description
            };

            await context.Transaction.AddAsync(transactionEntity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Transaction>> GetByFilterAsync(Guid accountId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var entities = await context.Transaction
            .AsNoTracking()
            .Include(t => t.CategoryEntity)
            .Include(t => t.BudgetEntity)
            .Where(transaction => transaction.BudgetEntity!.AccountId == accountId &&
                                  transaction.TimeTransaction >= startDate &&
                                  transaction.TimeTransaction <= endDate)
            .ToListAsync(cancellationToken);

        return MapGetByFilterAsync(entities);
    }

    public async Task<bool> ExistsAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var exist = await context.Transaction.AnyAsync(transaction => transaction.Id == transactionId, cancellationToken);

        return exist;
    }
    
    public async Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        await context.Transaction.Where(transaction => transaction.Id == transactionId).ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    private IEnumerable<Transaction> MapGetByFilterAsync(List<TransactionEntity> entities)
    {
        var transactions = new List<Transaction>();

        foreach (var entity in entities)
        {
            if (entity.CategoryEntity == null || entity.BudgetEntity == null)
                continue;

            if (!Enum.TryParse<TransactionType>(entity.TransactionTypeName, out var type))
                continue;

            var money = new Money(entity.Amount, entity.BudgetEntity.Currency);

            var category = new Category(entity.CategoryEntity.Name);
            typeof(Category)
                .GetField($"<{nameof(Category.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(category, entity.CategoryEntity.Id);

            var transaction = new Transaction(money, type, category, entity.TimeTransaction, entity.Description);
            typeof(Transaction)
                .GetField($"<{nameof(Transaction.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(transaction, entity.Id);

            transactions.Add(transaction);
        }

        return transactions;
    }
}