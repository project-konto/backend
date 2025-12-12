using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.DeleteBudget;

public record DeleteBudgetCommand(Guid BudgetId) : IRequest;