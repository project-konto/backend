using KontoApi.Application.Interfaces;
using KontoApi.Domain;


namespace KontoApi.Infrastructure;

public class UserRepository(KontoDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var userEntity = new Models.UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            HashedPassword = user.HashedPassword
        };

        await context.User.AddAsync(userEntity, cancellationToken);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await context.User.FindAsync(email);

        if (user == null)
            return null;

        return await UserDto(user);
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await context.User.FindAsync(userId, cancellationToken);

        if (user == null)
            return null;

        return await UserDto(user);
    }

    private async static Task<User> UserDto(Models.UserEntity user)
    {
        var dto = new User(user.Name, user.Email, user.HashedPassword);

        return dto;
    }
}
