using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface ITransactionRepository
{
	Task AddAsync(Transaction transaction);
	Task AddRangeAsync(IEnumerable<Transaction> transactions);
	Task<IEnumerable<Transaction>> GetByFilterAsync(Guid accountId, DateTime startDate, DateTime endDate);
	Task<bool> ExistsAsync(Guid transactionId);
	Task<bool> ExistsByExternalIdAsync(Guid UserId, string ExternalId);
	Task DeleteAsync(Guid transactionId);
}