namespace KontoApi.Application.Users.Transactions;

public class ImportTransactionCommand
{
    public Guid BudgetId { get; init; }
    public byte[] FileBytes { get; init; } = [];
    public string FileName { get; init; } = string.Empty;
    public string? ContentType { get; init; }
}