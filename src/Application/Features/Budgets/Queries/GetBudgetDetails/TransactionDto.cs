using KontoApi.Domain;

namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;

public record TransactionDto(
    Guid Id,
    decimal Amount,
    string Currency,
    TransactionType Type,
    string CategoryName,
    DateTime Date,
    string Description
);
