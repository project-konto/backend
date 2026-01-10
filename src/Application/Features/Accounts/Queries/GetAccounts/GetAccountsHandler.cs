using System.Linq;
using System.Collections.Generic;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;
using MediatR;

namespace KontoApi.Application.Features.Accounts.Queries.GetAccounts;

public class GetAccountsHandler(IAccountRepository accountRepository) : IRequestHandler<GetAccountsQuery, IEnumerable<AccountOverviewDto>?>
{
    public async Task<IEnumerable<AccountOverviewDto>?> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (accounts == null)
            return Enumerable.Empty<AccountOverviewDto>();

        return accounts.Select(account => new AccountOverviewDto(
            account.Id,
            account.Name,
            account.CreatedAt,
            account.Budgets.Select(b => new BudgetSummaryDto(
                b.Id,
                b.Name,
                b.CurrentBalance.Value,
                b.CurrentBalance.Currency
            )).ToList()
        ));
    }
}
