namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetsList;

public record BudgetSummaryDto(
    Guid Id,
    string Name,
    decimal CurrentBalance,
    string Currency
);