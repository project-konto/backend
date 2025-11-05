using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Infrastructure;

public class UserRepository : IUserRepository
{
    public Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<User?> FindByEmailAsync(String email)
    {
        throw new NotImplementedException();
    }
}
