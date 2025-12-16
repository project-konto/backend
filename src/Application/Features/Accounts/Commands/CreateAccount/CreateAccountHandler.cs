using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountHandler(IAccountRepository accountRepository, IUserRepository userRepository)
    : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null)
            throw new NotFoundException(typeof(User), request.UserId);

        var account = new Account(user, request.Name);

        await accountRepository.AddAsync(account, ct);

        return account.Id;
    }
}