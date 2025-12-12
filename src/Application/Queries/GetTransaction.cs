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