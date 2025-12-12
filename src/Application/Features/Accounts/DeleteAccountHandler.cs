using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Accounts;

public class DeleteAccountHandler(IAccountRepository accountRepository)
{
    public async Task Handle(Guid accountId, Guid userId, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (account is null || account.User.Id != userId)
        {
            throw new KeyNotFoundException($"Account {accountId} not found or access denied.");
        }

        await accountRepository.DeleteAsync(accountId, cancellationToken);
    }
}