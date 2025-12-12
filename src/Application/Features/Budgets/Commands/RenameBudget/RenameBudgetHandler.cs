using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.RenameBudget;

public class RenameBudgetHandler : IRequestHandler<RenameBudgetCommand>
{
    private readonly IBudgetRepository budgetRepository;

    public RenameBudgetHandler(IBudgetRepository budgetRepository)
    {
        this.budgetRepository = budgetRepository;
    }

    public async Task Handle(RenameBudgetCommand request, CancellationToken ct)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, ct);
        if (budget == null) throw new NotFoundException(typeof(Budget), request.BudgetId);

        budget.Rename(request.NewName);

        await budgetRepository.UpdateAsync(budget, ct);
    }
}