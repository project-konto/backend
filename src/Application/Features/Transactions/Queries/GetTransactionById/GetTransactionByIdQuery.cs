using MediatR;

namespace KontoApi.Application.Features.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid Id) : IRequest<TransactionDetailDto>;