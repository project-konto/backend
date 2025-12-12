using KontoApi.Domain;

namespace KontoApi.Application.Users.Transactions;

public class AddTransactionCommand
{
    public Guid BudgetId { get; init; }
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "";
    public string Category { get; init; } = "";
    public DateTime Date { get; init; }
    public string? Description { get; init; }
}