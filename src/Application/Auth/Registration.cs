using KontoApi.Application.DTOs;
using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Users;

public class RegisterUserCommand
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }

    public RegisterUserCommand(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}

public class RegisterUserHandler
{
    private readonly IAuthService authService;
    private readonly IUserRepository usersRepository;

    public RegisterUserHandler(IAuthService authService, IUserRepository usersRepository)
    {
        this.authService = authService;
        this.usersRepository = usersRepository;
    }

    public async Task<AuthUserDto> Handle(RegisterUserCommand command)
    {
        if (await authService.FindUserByEmailAsync(command.Email) != null)
            throw new ConflictException($"User with email {command.Email} already exists");

        var user = new User(command.Name, command.Email, authService.HashPassword(command.Password));
        await usersRepository.AddAsync(user);

        return authService.CreateAuthResult(user);
    }
}