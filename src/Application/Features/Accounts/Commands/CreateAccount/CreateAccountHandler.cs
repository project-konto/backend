using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KontoApi.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountHandler(IAccountRepository accountRepository, IUserRepository userRepository, Microsoft.Extensions.Logging.ILogger<CreateAccountHandler> logger)
    : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct);
        if (user == null)
            throw new NotFoundException(typeof(User), request.UserId);

        var account = new Account(user, request.Name);

        logger.LogInformation("Creating account for UserId={UserId}, TempAccountId={AccountId}", request.UserId, account.Id);
        await accountRepository.AddAsync(account, ct);
        logger.LogInformation("Created account Id={AccountId} for UserId={UserId}", account.Id, request.UserId);

        return account.Id;
    }
}