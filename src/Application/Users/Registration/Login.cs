using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Users;

public class LoginUserCommand
{
    public string Email { get; init; }
    public string Password { get; init; }
}

public class LoginUserDto
{
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}

public class LoginUserHandler
{
    private readonly IUserRepository usersRepository;
    private readonly IPasswordHasher passwordHasher;

    public LoginUserHandler(IUserRepository users, IPasswordHasher hasher)
    {
        usersRepository = users;
        passwordHasher = hasher;
    }

    public async Task<LoginUserDto> Handle(LoginUserCommand command)
    {
        var user = await usersRepository.FindByEmailAsync(command.Email);
        if (user == null)
            throw new InvalidOperationException("Email or password is incorrect");

        var hash = user.HashedPassword;
        if (!passwordHasher.Verify(command.Password, hash))
            throw new InvalidOperationException("Invalid email or password");

        return new()
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}