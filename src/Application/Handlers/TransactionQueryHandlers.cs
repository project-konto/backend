using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Application.Queries;

namespace KontoApi.Application.Handlers;

public class GetTransactionsHandler
{
    private readonly ITransactionRepository transactionRepository;

    public GetTransactionsHandler(ITransactionRepository repository) => transactionRepository = repository;

    public async Task<IEnumerable<TransactionListDto>> Handle(GetTransactionsQuery query)
    {
        var range = query.DateRange ?? DateRange.Create(null, null);
        var transactions = await transactionRepository.GetByFilterAsync(query.AccountId, range.Start, range.End);
        var filtered = transactions.AsQueryable();

        if (query.Type.HasValue)
            filtered = filtered.Where(t => t.Type == query.Type.Value);

        if (!string.IsNullOrWhiteSpace(query.Category))
            filtered = filtered.Where(t => t.TransactionCategory.Name == query.Category);

        if (query.MinAmount.HasValue)
            filtered = filtered.Where(t => t.Amount.Value >= query.MinAmount.Value);

        if (query.MaxAmount.HasValue)
            filtered = filtered.Where(t => t.Amount.Value <= query.MaxAmount.Value);

        return filtered.Select(t => new TransactionListDto
        {
            Id = t.Id,
            Type = t.Type,
            Amount = t.Amount.Value,
            Currency = t.Amount.Currency,
            Category = t.TransactionCategory.Name,
            Date = t.Date,
            Description = t.Description
        });
    }
}