using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.DeleteBudget;

public class DeleteBudgetHandler : IRequestHandler<DeleteBudgetCommand>
{
    private readonly IBudgetRepository budgetRepository;

    public DeleteBudgetHandler(IBudgetRepository budgetRepository)
    {
        this.budgetRepository = budgetRepository;
    }

    public async Task Handle(DeleteBudgetCommand request, CancellationToken ct)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, ct);
        if (budget == null) throw new NotFoundException(typeof(Budget), request.BudgetId);

        await budgetRepository.DeleteAsync(request.BudgetId, ct);
    }
}