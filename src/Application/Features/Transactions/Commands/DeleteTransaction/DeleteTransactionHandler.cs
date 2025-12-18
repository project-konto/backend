using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionHandler(IBudgetRepository budgetRepository) : IRequestHandler<DeleteTransactionCommand>
{
    public async Task Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, cancellationToken);
        if (budget == null)
            throw new NotFoundException(typeof(Budget), request.BudgetId);

        try
        {
            await budgetRepository.DeleteTransactionAsync(request.BudgetId, request.TransactionId, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException(typeof(Transaction), request.TransactionId);
        }
    }
}