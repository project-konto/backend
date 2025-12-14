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
