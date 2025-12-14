using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Budget budget, CancellationToken cancellationToken);

    Task UpdateAsync(Budget budget, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
