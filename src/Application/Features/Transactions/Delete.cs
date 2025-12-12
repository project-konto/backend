namespace KontoApi.Application.Users.Transactions;

public class DeleteTransactionCommand
{
    public Guid BudgetId { get; init; }
    public Guid TransactionId { get; init; }
}