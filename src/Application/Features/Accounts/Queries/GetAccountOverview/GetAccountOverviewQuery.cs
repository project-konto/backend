using MediatR;

namespace KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;

public record GetAccountOverviewQuery(Guid UserId) : IRequest<AccountOverviewDto>;
