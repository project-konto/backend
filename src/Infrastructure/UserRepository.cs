using KontoApi.Application.Interfaces;
using KontoApi.Domain;


namespace KontoApi.Infrastructure;

public class UserRepository : IUserRepository
{
    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
