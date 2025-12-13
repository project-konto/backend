using MediatR;

namespace KontoApi.Application.Features.Transactions.Commands.ImportTransactions;

public record ImportTransactionsCommand(
    Guid BudgetId,
    Stream FileStream,
    string FileName
) : IRequest<ImportResultDto>;