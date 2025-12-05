using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Infrastructure;

public class TransactionRepository : ITransactionRepository
{
    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Transaction>> GetByFilterAsync(Guid accountId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByExternalIdAsync(Guid UserId, string ExternalId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}