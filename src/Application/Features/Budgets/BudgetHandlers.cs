using KontoApi.Application.DTOs;
using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Application.Queries;

namespace KontoApi.Application.Handlers;

public class GetBudgetHandler
{
    private readonly IBudgetRepository budgetRepository;

    public GetBudgetHandler(IBudgetRepository repository) => budgetRepository = repository;

    public async Task<BudgetDto> Handle(GetBudgetQuery query)
    {
        // Fix to accept BudgetId
        var budget = await budgetRepository.GetByIdAsync(query.UserId, CancellationToken.None);
        if (budget == null)
            throw new NotFoundException("Budget not found");

        return new()
        {
            Id = budget.Id,
            CurrentBalance = budget.CurrentBalance.Value,
            Currency = budget.CurrentBalance.Currency,
            TransactionsCount = budget.Transactions.Count
        };
    }
}