using KontoApi.Application.Exceptions;
using KontoApi.Application.Features.Accounts.Commands.DeleteAccount;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Accounts;

public class DeleteAccountHandler(IAccountRepository accountRepository) : IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account == null)
        {
            throw new NotFoundException(typeof(Account), request.AccountId);
        }

        await accountRepository.DeleteAsync(request.AccountId, ct);
    }
}
