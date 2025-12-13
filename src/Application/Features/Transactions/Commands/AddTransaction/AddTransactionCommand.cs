using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Transactions.Commands.AddTransaction;

public record AddTransactionCommand(
	Guid BudgetId,
	decimal Amount,
	string Currency,
	TransactionType Type,
	Guid CategoryId,
	DateTime Date,
	string? Description
) : IRequest<Guid>;