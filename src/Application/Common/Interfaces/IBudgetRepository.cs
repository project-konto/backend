using KontoApi.Domain;

namespace KontoApi.Application.Common.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Budget budget, CancellationToken cancellationToken);
    Task UpdateAsync(Budget budget, CancellationToken cancellationToken);
    Task AddTransactionAsync(Guid budgetId, Transaction transaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task DeleteTransactionAsync(Guid budgetId, Guid transactionId, CancellationToken cancellationToken = default);
}