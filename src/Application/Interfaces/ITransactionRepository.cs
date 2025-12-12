using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByFilterAsync(Guid accountId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid transactionId, CancellationToken cancellationToken = default);
}