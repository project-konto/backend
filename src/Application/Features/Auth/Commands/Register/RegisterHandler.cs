using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Auth.Commands.Register;

public class RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand, Guid>
{
    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
            throw new ConflictException($"Email '{request.Email}' is already in use");

        var hashedPassword = passwordHasher.Hash(request.Password);

        var user = new User(request.Name, request.Email, hashedPassword);

        await userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}