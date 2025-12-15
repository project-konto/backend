using MediatR;

namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetsList;

public record GetBudgetsListQuery(Guid AccountId) : IRequest<List<BudgetSummaryDto>>;