using MediatR;

namespace KontoApi.Application.Features.Accounts.Queries.GetAccounts;

public record GetAccountsQuery(Guid UserId) : IRequest<IEnumerable<KontoApi.Application.Features.Accounts.Queries.GetAccountOverview.AccountOverviewDto>?>;
