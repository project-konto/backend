using KontoApi.Domain;

namespace KontoApi.Application.Features.Transactions.Queries.GetTransactionById;

public record TransactionDetailDto(
    Guid Id,
    decimal Amount,
    string Currency,
    TransactionType Type,
    Guid CategoryId,
    string CategoryName,
    DateTime Date,
    string Description
);
