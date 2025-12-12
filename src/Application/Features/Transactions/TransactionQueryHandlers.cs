using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Application.Queries;

namespace KontoApi.Application.Handlers;

public class GetTransactionsHandler
{
    private readonly IBudgetRepository budgetRepository;

    public GetTransactionsHandler(IBudgetRepository budgetRepository)
        => this.budgetRepository = budgetRepository;

    public async Task<IEnumerable<TransactionResponse>> Handle(GetTransactionsQuery query)
    {
        // TODO

        // var range = query.DateRange ?? DateRange.Create(null, null);
        // var transactions = await transactionRepository.GetByFilterAsync(query.BudgetId, range.StartDate, range.EndDate);
        // // var filtered = transactions.AsQueryable();
        //
        // if (query.Type.HasValue)
        //     filtered = filtered.Where(t => t.Type == query.Type.Value);
        //
        // if (!string.IsNullOrWhiteSpace(query.Category))
        //     filtered = filtered.Where(t => t.TransactionCategory.Name == query.Category);
        //
        // if (query.MinAmount.HasValue)
        //     filtered = filtered.Where(t => t.Amount.Value >= query.MinAmount.Value);
        //
        // if (query.MaxAmount.HasValue)
        //     filtered = filtered.Where(t => t.Amount.Value <= query.MaxAmount.Value);
        //
        // return filtered.Select(t => new TransactionResponse
        // {
        //     Id = t.Id,
        //     Amount = (double)t.Amount.Value,
        //     Currency = t.Amount.Currency,
        //     Category = t.TransactionCategory.Name,
        //     Date = t.Date,
        //     Description = t.Description,
        //     Type = t.Type.ToString()
        // });

        return null;
    }
}