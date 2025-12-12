namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;

public record BudgetDetailsDto(
    Guid Id,
    string Name,
    decimal CurrentBalance,
    string Currency,
    List<TransactionDto> Transactions
);