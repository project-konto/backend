using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Queries;

public class GetBudget
{
    public class GetBudgetQuery
    {
        public Guid UserId { get; set; }
    }

    public class BudgetDto
    {
        public Guid Id { get; init; }
        public decimal CurrentBalance { get; init; }
        public string Currency { get; init; } = "";
        public int TransactionsCount { get; init; }
    }

    public class GetBudgetHandler
    {
        private readonly IBudgetRepository budgetRepository;

        public GetBudgetHandler(IBudgetRepository repository) => budgetRepository = repository;

        public async Task<BudgetDto> Handle(GetBudgetQuery query)
        {
            var budget = await budgetRepository.GetByUserIdAsync(query.UserId);
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
}