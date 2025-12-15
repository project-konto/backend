using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountHandler(IAccountRepository accountRepository, IUserRepository userRepository)
    : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(typeof(User), request.UserId);

        var existingAccounts = await accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existingAccounts.Any())
            throw new ConflictException("User already has an account");

        var account = new Account(user);

        await accountRepository.AddAsync(account, cancellationToken);

        return account.Id;
    }
}