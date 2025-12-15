using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.RenameBudget;

public class RenameBudgetHandler(IBudgetRepository budgetRepository) : IRequestHandler<RenameBudgetCommand>
{
    public async Task Handle(RenameBudgetCommand request, CancellationToken ct)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, ct);
        if (budget == null) throw new NotFoundException(typeof(Budget), request.BudgetId);

        budget.Rename(request.NewName);

        await budgetRepository.UpdateAsync(budget, ct);
    }
}