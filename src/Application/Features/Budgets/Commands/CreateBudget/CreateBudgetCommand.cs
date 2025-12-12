using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.CreateBudget;

public record CreateBudgetCommand(
    Guid AccountId,
    string Name,
    decimal InitialBalance,
    string Currency
) : IRequest<Guid>;