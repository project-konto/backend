using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid accountId, CancellationToken cancellationToken = default);
}