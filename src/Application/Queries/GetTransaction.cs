using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Queries;

public class GetTransactionsQuery
{
    public Guid AccountId { get; init; }
    public DateRange? DateRange { get; init; }
    public TransactionType? Type { get; init; }
    public string? Category { get; init; }
    public decimal? MinAmount { get; init; }
    public decimal? MaxAmount { get; init; }
}

public class TransactionDto
{
    public Guid Id { get; init; }
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "";
    public string Category { get; init; } = "";
    public DateTime Date { get; init; }
    public string Description { get; init; } = "";
}

public record DateRange(DateTime Start, DateTime End)
{
    public static DateRange Create(DateTime? start, DateTime? end)
    {
        var from = start ?? DateTime.MinValue;
        var to = end ?? DateTime.MaxValue;

        if (from > to)
            throw new ValidationException("Start date must be before end date");
        return new(from, to);
    }
}

public class GetTransactionsHandler
{
    private readonly ITransactionRepository transactionRepository;

    public GetTransactionsHandler(ITransactionRepository repository) => transactionRepository = repository;

    public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionsQuery query)
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

        return filtered.Select(t => new TransactionDto
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