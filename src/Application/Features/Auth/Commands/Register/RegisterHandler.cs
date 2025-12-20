using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using MediatR;

namespace KontoApi.Application.Features.Auth.Commands.Register;

public class RegisterHandler(IUserRepository userRepository, 
    IPasswordHasher passwordHasher, ISender sender) : IRequestHandler<RegisterCommand, Guid>
{
    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
            throw new ConflictException($"Email '{request.Email}' is already in use");

        var hashedPassword = passwordHasher.Hash(request.Password);
        var user = new Domain.User(request.Name, request.Email, hashedPassword);

        await userRepository.AddAsync(user, cancellationToken);
        await sender.Send(new Accounts.Commands.CreateAccount.CreateAccountCommand(user.Id, "Default"), cancellationToken);
        return user.Id;
    }
}