using KontoApi.Application.Features.Transactions.Queries.Common;
using MediatR;

namespace KontoApi.Application.Features.Transactions.Queries.GetTransactions;

public record GetTransactionsQuery(
    Guid BudgetId,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<List<TransactionDetailDto>>;