using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Users;

public class RegisterUserCommand
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}

public class RegisterUserDto
{
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}

public class RegisterUserHandler
{
    private readonly IUserRepository usersRepository;
    private readonly IPasswordHasher passwordHasher;

    public RegisterUserHandler(IUserRepository users, IPasswordHasher hasher)
    {
        usersRepository = users;
        passwordHasher = hasher;
    }

    public async Task<RegisterUserDto> Handle(RegisterUserCommand command)
    {
        var existingUser = await usersRepository.FindByEmailAsync(command.Email);
        if (existingUser != null)
            throw new InvalidOperationException($"User with email {command.Email} already exists");

        var hashed = passwordHasher.Hash(command.Password);
        var user = new User(command.Name, command.Email, hashed);

        await usersRepository.AddAsync(user);
        return new()
        {
            Name = user.Name,
            Email = user.Email
        };
    }
}