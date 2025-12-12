using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Guid budgetId, Transaction transaction, CancellationToken cancellationToken = default);
    Task AddRangeAsync(Guid budgetId, IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid budgetId, Guid transactionId, CancellationToken cancellationToken = default);
}