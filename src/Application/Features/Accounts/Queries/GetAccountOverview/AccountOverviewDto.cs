namespace KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;

public record AccountOverviewDto(
    Guid Id,
    string OwnerName,
    DateTime CreatedAt,
    List<BudgetSummaryDto> Budgets
);

public record BudgetSummaryDto(
    Guid Id,
    string Name,
    decimal Balance,
    string Currency
);