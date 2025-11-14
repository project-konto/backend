using KontoApi.Application.Interfaces;
using KontoApi.Domain;
namespace KontoApi.Infrastructure;

public class BudgetRepository : IBudgetRepository
{
    public Task<Budget?> GetByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}