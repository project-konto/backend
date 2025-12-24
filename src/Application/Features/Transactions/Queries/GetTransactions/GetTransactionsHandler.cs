using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Features.Transactions.Queries.Common;
using KontoApi.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Features.Transactions.Queries.GetTransactions;

public class GetTransactionsHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetTransactionsQuery, List<TransactionDetailDto>>
{
    public async Task<List<TransactionDetailDto>> Handle(GetTransactionsQuery request, CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        var hasAccess = await dbContext.Accounts
            .AnyAsync(a => a.User.Id == userId &&
                           a.Budgets.Any(b => b.Id == request.BudgetId), ct);

        if (!hasAccess)
            throw new NotFoundException(typeof(Budget), request.BudgetId);

        var query = dbContext.Transactions
            .AsNoTracking()
            .Where(t => EF.Property<Guid>(t, "BudgetId") == request.BudgetId);

        if (request.FromDate.HasValue)
            query = query.Where(t => t.Date >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(t => t.Date <= request.ToDate.Value);

        return await query
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionDetailDto(
                t.Id,
                t.Amount.Value,
                t.Amount.Currency,
                t.Type,
                t.TransactionCategory.Id,
                t.TransactionCategory.Name,
                t.Date,
                t.Description ?? ""
            ))
            .ToListAsync(ct);
    }
}