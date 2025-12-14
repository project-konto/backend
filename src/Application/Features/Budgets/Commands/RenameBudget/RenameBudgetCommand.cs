using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.RenameBudget;

public record RenameBudgetCommand(Guid BudgetId, string NewName) : IRequest;
