using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;

public class GetAccountOverviewHandler(IAccountRepository accountRepository, Microsoft.Extensions.Logging.ILogger<GetAccountOverviewHandler> logger)
    : IRequestHandler<GetAccountOverviewQuery, AccountOverviewDto>
{
    public async Task<AccountOverviewDto> Handle(GetAccountOverviewQuery request, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var account = accounts.FirstOrDefault();
        if (account == null)
        {
            logger.LogInformation("GetAccountOverview: no account found for UserId={UserId}", request.UserId);
            throw new NotFoundException("Account not found for this user.");
        }
        logger.LogInformation("GetAccountOverview: returning account Id={AccountId} for UserId={UserId}", account.Id, request.UserId);

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