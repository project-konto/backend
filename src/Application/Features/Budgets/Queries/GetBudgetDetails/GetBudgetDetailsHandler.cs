using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;

public class GetBudgetDetailsHandler : IRequestHandler<GetBudgetDetailsQuery, BudgetDetailsDto>
{
    private readonly IBudgetRepository budgetRepository;

    public GetBudgetDetailsHandler(IBudgetRepository budgetRepository)
        => this.budgetRepository = budgetRepository;

    public async Task<BudgetDetailsDto> Handle(GetBudgetDetailsQuery request, CancellationToken ct)
    {
        // Repository includes Transactions and Categories automatically
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, ct);

        if (budget == null) throw new NotFoundException(typeof(Budget), request.BudgetId);

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
