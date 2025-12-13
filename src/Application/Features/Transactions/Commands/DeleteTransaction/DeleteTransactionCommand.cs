using MediatR;

namespace KontoApi.Application.Features.Transactions.Commands.DeleteTransaction;

public record DeleteTransactionCommand(Guid BudgetId, Guid TransactionId) : IRequest;