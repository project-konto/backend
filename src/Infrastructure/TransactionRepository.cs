using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Infrastructure;

public class TransactionRepository : ITransactionRepository
{
    public Task AddAsync(Transaction transaction)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<Transaction> transactions)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Transaction>> GetByFilterAsync(Guid accountId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public Task<Boolean> ExistsAsync(Guid transactionId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid transactionId)
    {
        throw new NotImplementedException();
    }

    public Task<Boolean> ExistsByExternalIdAsync(Guid UserId, String ExternalId)
    {
        throw new NotImplementedException();
    }
}