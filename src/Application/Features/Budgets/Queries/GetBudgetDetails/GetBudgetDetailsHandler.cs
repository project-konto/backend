using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;

public class GetBudgetDetailsHandler(IBudgetRepository budgetRepository)
    : IRequestHandler<GetBudgetDetailsQuery, BudgetDetailsDto>
{
    public async Task<BudgetDetailsDto> Handle(GetBudgetDetailsQuery request, CancellationToken ct)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, ct);
        if (budget == null)
            throw new NotFoundException(typeof(Budget), request.BudgetId);

        return new(
            budget.Id,
            budget.Name,
            budget.CurrentBalance.Value,
            budget.CurrentBalance.Currency,
            budget.Transactions.Select(t => new TransactionDto(
                t.Id,
                t.Amount.Value,
                t.Amount.Currency,
                t.Type,
                t.TransactionCategory.Name,
                t.Date,
                t.Description ?? string.Empty
            )).OrderByDescending(t => t.Date).ToList()
        );
    }
}