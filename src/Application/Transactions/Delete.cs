namespace KontoApi.Application.Users.Transactions;

public class DeleteTransactionCommand
{
    public Guid UserId { get; init; }
    public Guid TransactionId { get; init; }
}