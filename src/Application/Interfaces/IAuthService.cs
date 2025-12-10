using KontoApi.Application.DTOs;
using KontoApi.Domain;

namespace KontoApi.Application.Interfaces;

public interface IAuthService
{
    Task<User?> FindUserByEmailAsync(string email);
    bool VerifyPassword(string password, string hash);
    string HashPassword(string password);
    AuthUserDto CreateAuthResult(User user);
}