using MediatR;

namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;

public record GetBudgetDetailsQuery(Guid BudgetId) : IRequest<BudgetDetailsDto>;
