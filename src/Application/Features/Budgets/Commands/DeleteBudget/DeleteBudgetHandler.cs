using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.DeleteBudget;

public class DeleteBudgetHandler(IBudgetRepository budgetRepository) : IRequestHandler<DeleteBudgetCommand>
{
    public async Task Handle(DeleteBudgetCommand request, CancellationToken ct)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, ct);
        if (budget == null) throw new NotFoundException(typeof(Budget), request.BudgetId);

        await budgetRepository.DeleteAsync(request.BudgetId, ct);
    }
}