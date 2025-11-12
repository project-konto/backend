using KontoApi.Application.Interfaces;

namespace KontoApi.Infrastructure;

public class BudgetRepository : IBudgetRepository
{
    public Task<Budget?> GetByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}