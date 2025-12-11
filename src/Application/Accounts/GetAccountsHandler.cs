using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Accounts;

public class GetAccountsHandler(IAccountRepository accountRepository)
{
    public async Task<IEnumerable<AccountDto>> Handle(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await accountRepository.GetByUserIdAsync(userId, cancellationToken);
        var itemsDto = items.Select(x => new AccountDto()
        {
            BudgetsCount = x.Budgets.Count,
            UserId = x.User.Id,
            Id = x.Id,
            CreatedAt = x.CreatedAt,
        });

        return itemsDto;
    }
}