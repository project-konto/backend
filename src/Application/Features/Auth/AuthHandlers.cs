using KontoApi.Application.DTOs;
using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Application.Users;
using KontoApi.Domain;

namespace KontoApi.Application.Handlers;

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
        await usersRepository.AddAsync(user, CancellationToken.None);

        return authService.CreateAuthResult(user);
    }
}

public class LoginUserHandler
{
    private readonly IAuthService authService;

    public LoginUserHandler(IAuthService authService)
        => this.authService = authService;

    public async Task<AuthUserDto> Handle(LoginUserCommand command)
    {
        var user = await authService.FindUserByEmailAsync(command.Email);

        if (user == null || !authService.VerifyPassword(command.Password, user.HashedPassword))
            throw new UnauthorizedException("Email or password is incorrect");

        return authService.CreateAuthResult(user);
    }
}

public class LogoutUserHandler
{
    public Task Handle(LogoutUserCommand command) => Task.CompletedTask;
}
