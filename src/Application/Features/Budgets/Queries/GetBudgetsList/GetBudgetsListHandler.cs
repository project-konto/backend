using KontoApi.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Features.Budgets.Queries.GetBudgetsList;

public class GetBudgetsListHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBudgetsListQuery, List<BudgetSummaryDto>>
{
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