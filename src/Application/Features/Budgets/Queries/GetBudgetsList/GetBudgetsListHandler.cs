using KontoApi.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetsList;

public class GetBudgetsListHandler : IRequestHandler<GetBudgetsListQuery, List<BudgetSummaryDto>>
{
    private readonly IApplicationDbContext dbContext;

    public GetBudgetsListHandler(IApplicationDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<List<BudgetSummaryDto>> Handle(GetBudgetsListQuery request, CancellationToken ct)
    {
        return await dbContext.Budgets
            .AsNoTracking()
            .Where(b => EF.Property<Guid>(b, "AccountId") == request.AccountId)
            .Select(b => new BudgetSummaryDto(
                b.Id,
                b.Name,
                b.CurrentBalance.Value,
                b.CurrentBalance.Currency
            ))
            .ToListAsync(ct);
    }
}