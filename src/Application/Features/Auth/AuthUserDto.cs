using KontoApi.Domain;

namespace KontoApi.Application.DTOs;

public class AuthUserDto
{
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string Token { get; init; }

    public AuthUserDto(Guid userId, string name, string email, string token)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Token = token;
    }

    public AuthUserDto(User user, string token)
    {
        UserId = user.Id;
        Name = user.Name;
        Email = user.Email;
        Token = token;
    }
}