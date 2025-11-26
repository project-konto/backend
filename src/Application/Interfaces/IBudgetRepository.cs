using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface IBudgetRepository
{
	Task<Budget?> GetByUserIdAsync(Guid userId);
}