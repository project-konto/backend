using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using MediatR;

namespace KontoApi.Application.Features.Auth.Commands.Login;

public class LoginHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !passwordHasher.Verify(request.Password, user.HashedPassword))
            throw new UnauthorizedException("Invalid email or password");

        var token = jwtProvider.Generate(user);

        return new(token, user.Id, user.Name, user.Email);
    }
}