using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Features.Transactions.Queries.Common;
using KontoApi.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetTransactionByIdQuery, TransactionDetailDto>
{
    public async Task<TransactionDetailDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await dbContext.Transactions
            .AsNoTracking()
            .Include(x => x.TransactionCategory)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (transaction == null)
            throw new NotFoundException(typeof(Transaction), request.Id);

        return new(
            transaction.Id,
            transaction.Amount.Value,
            transaction.Amount.Currency,
            transaction.Type,
            transaction.TransactionCategory.Id,
            transaction.TransactionCategory.Name,
            transaction.Date,
            transaction.Description ?? string.Empty
        );
    }
}