using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByFilterAsync(Guid accountId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByExternalIdAsync(Guid UserId, string ExternalId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken = default);
}