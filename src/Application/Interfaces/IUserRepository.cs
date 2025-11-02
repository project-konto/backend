namespace KontoApi.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> FindByEmailAsync(string email); // checks if email exists
}