using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Accounts;

public class CreateAccountHandler(
    IAccountRepository accountRepository,
    IUserRepository userRepository)
{
    public async Task<AccountDto> Handle(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        var account = new Account(user);
        var budgetsCount = account.Budgets.Count;

        await accountRepository.AddAsync(account, cancellationToken);

        return new AccountDto
        {
            BudgetsCount = budgetsCount,
            UserId = user.Id,
            CreatedAt = account.CreatedAt,
            Id = account.Id
        };
    }
}