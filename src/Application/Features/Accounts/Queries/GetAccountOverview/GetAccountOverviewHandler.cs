using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;

public class GetAccountOverviewHandler : IRequestHandler<GetAccountOverviewQuery, AccountOverviewDto>
{
    private readonly IAccountRepository accountRepository;

    public GetAccountOverviewHandler(IAccountRepository accountRepository)
    {
        this.accountRepository = accountRepository;
    }

    public async Task<AccountOverviewDto> Handle(GetAccountOverviewQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var account = accounts.FirstOrDefault();

        if (account == null)
        {
            throw new NotFoundException("Account not found for this user.");
        }

        return new AccountOverviewDto(
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
