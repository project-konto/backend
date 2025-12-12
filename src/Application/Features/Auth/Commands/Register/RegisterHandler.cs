using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher passwordHasher;

    public RegisterHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
    }

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