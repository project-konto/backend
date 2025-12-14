using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using MediatR;

namespace KontoApi.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly ITokenService tokenService;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !passwordHasher.Verify(request.Password, user.HashedPassword))
            throw new UnauthorizedException("Invalid email or password");

        var token = tokenService.GenerateAccessToken(user);

        return new(token, user.Id, user.Name, user.Email);
    }
}
