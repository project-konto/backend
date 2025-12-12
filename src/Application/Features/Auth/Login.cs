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