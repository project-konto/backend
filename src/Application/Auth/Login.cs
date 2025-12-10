using KontoApi.Application.DTOs;
using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Users;

public class LoginUserCommand
{
    public string Email { get; init; }
    public string Password { get; init; }

    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
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