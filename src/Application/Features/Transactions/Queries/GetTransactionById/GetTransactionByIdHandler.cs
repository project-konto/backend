using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDetailDto>
{
    private readonly IApplicationDbContext dbContext;

    public GetTransactionByIdHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<TransactionDetailDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await dbContext.Transactions
            .AsNoTracking()
            .Include(x => x.TransactionCategory)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (transaction == null)
            throw new NotFoundException(typeof(Transaction), request.Id);

        return new TransactionDetailDto(
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
