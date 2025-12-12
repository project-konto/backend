using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository usersRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly ITokenService tokenService;

    public AuthService(IUserRepository usersRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        this.usersRepository = usersRepository;
        this.passwordHasher = passwordHasher;
        this.tokenService = tokenService;
    }

    public Task<User?> FindUserByEmailAsync(string email)
        => usersRepository.GetByEmailAsync(email, CancellationToken.None);

    public bool VerifyPassword(string password, string hash)
        => passwordHasher.Verify(password, hash);
    public string HashPassword(string password)
        => passwordHasher.Hash(password);
    public AuthUserDto CreateAuthResult(User user)
        => new(user, tokenService.GenerateAccessToken(user));
}