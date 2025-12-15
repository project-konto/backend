using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using MediatR;

namespace KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;

public class GetAccountOverviewHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAccountOverviewQuery, AccountOverviewDto>
{
    public async Task<AccountOverviewDto> Handle(GetAccountOverviewQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var account = accounts.FirstOrDefault();
        if (account == null)
            throw new NotFoundException("Account not found for this user.");

        return new(
            account.Id,
            account.User.Name,
            account.CreatedAt,
            account.Budgets.Select(b => new BudgetSummaryDto(
                b.Id,
                b.Name,
                b.CurrentBalance.Value,
                b.CurrentBalance.Currency
            )).ToList()
        );
    }
}