namespace KontoApi.Application.Features.Transactions.Commands.ImportTransactions;

public record ImportResultDto(
    int TotalProcessed,
    int SuccessCount,
    int FailedCount,
    List<string> Errors
);